using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameEvents;
using Newtonsoft.Json;
using QFramework;
using Runtime.Business.Data;
using Runtime.Business.Data.Entry;
using Runtime.Business.Manager;
using Runtime.Business.Util;
using UI;
using UnityEngine;

namespace GameEvents
{
    public class SetBuildingState : GameEventBaseNoDefaultCreate<SetBuildingState>
    {
        public bool State;
        public Action OnStateChanged;

        public static SetBuildingState Create(bool state, Action onStateChanged)
        {
            var self = Create();
            self.State = state;
            self.OnStateChanged = onStateChanged;
            return self;
        }
    }

    public class AddCardToDeck : GameEventBaseNoDefaultCreate<AddCardToDeck>
    {
        public string CardId;

        public static AddCardToDeck Create(string cardId)
        {
            var self = Create();
            self.CardId = cardId;
            return self;
        }
    }

    public class RemoveCardFromDeck : GameEventBaseNoDefaultCreate<RemoveCardFromDeck>
    {
        public string CardId;

        public static RemoveCardFromDeck Create(string cardId)
        {
            var self = Create();
            self.CardId = cardId;
            return self;
        }
    }

    public class SaveDeck : GameEventBase<SaveDeck>
    {
        
    }

    public class DeleteDeck : GameEventBaseNoDefaultCreate<DeleteDeck>
    {
        public string Name;

        public static DeleteDeck Create(string name)
        {
            var self = Create();
            self.Name = name;
            return self;
        }
    }

    public class EditDeck : GameEventBaseNoDefaultCreate<EditDeck>
    {
        public string Name;

        public static EditDeck Create(string name)
        {
            var self = Create();
            self.Name = name;
            return self;
        }
    }
}

namespace Logic
{
    public class BuildDeckLogic : LogicBase
    {
        public bool IsBuilding { get; private set; }

        /// <summary>
        /// 临时储存
        /// </summary>
        public List<string> CardIds { get; private set; } = new();

        /// <summary>
        /// json长期储存
        /// </summary>
        public Dictionary<string, BuildDeckEntry> DeckEntries { get; private set; }

        private CardEntry _leaderCard;
        private int _triggerCardCount;
        private HashSet<ElementAttribute> _cardAttributes = new();

        protected override void RegisterEvents()
        {
            base.RegisterEvents();

            var ec = GetEventComponent();
            ec.Listen<AddCardToDeck>(OnAddCard);
            ec.Listen<RemoveCardFromDeck>(OnRemoveCard);
            ec.Listen<SetBuildingState>(e =>
            {
                IsBuilding = e.State;
                e.OnStateChanged?.Invoke();
            });
            ec.Listen<SaveDeck>(OnSave);
            ec.Listen<DeleteDeck>(OnDelete);

            var filePath = Path.Combine(Application.persistentDataPath, "decks.json");
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

            var json = File.ReadAllText(filePath);
            DeckEntries = JsonConvert.DeserializeObject<Dictionary<string, BuildDeckEntry>>(json);
        }

        private void OnAddCard(AddCardToDeck e)
        {
            if (CardIds.Count == 40)
            {
                SendTips("卡牌数量不能超过40张!");
                return;
            }

            var id = e.CardId;
            var cardEntry = DataManager.Instance.GetCard(id);
            if (cardEntry.CardType is CardType.Leader)
            {
                if (_leaderCard != null)
                {
                    SendTips("领袖卡牌只能选择1张!");
                }
                else
                {
                    if (cardEntry.Skills[0] == 10001)
                    {
                        if (_cardAttributes.Any(attribute => attribute != cardEntry.Attribute))  // 有其他属性的卡
                        {
                            SendTips($"你的预设卡组中有与领袖卡属性不同的卡牌，是否要移除所有非<sprite name=\"{cardEntry.SkillParams[0]}\">的卡牌?",
                                () =>
                                {
                                    for (int i = CardIds.Count - 1; i >= 0; i--)
                                    {
                                        var entry = DataManager.Instance.GetCard(CardIds[i]);
                                        if (entry.Attribute != cardEntry.Attribute)
                                        {
                                            CardIds.RemoveAt(i);
                                        }
                                    }
                                    
                                    _leaderCard = cardEntry;
                                    CardIds.Add(id);
                                }, null);
                        }
                        else
                        {
                            _leaderCard = cardEntry;
                            CardIds.Add(id);
                        }
                    }
                    else
                    {
                        var diffCount = _cardAttributes.Count(attribute => attribute != cardEntry.Attribute);
                        if (diffCount > 1)
                        {
                            SendTips($"当前领袖牌只允许有1种除<sprite name=\"{cardEntry.SkillParams[0]}\">属性的卡，请先移除多出的{diffCount - 1}种。");
                            return;
                        }
                    }
                }

                return;
            }

            if (CardIds.Count == 39 && _leaderCard == null)
            {
                SendTips("非领袖卡已达上限!");
                return;
            }

            if (_leaderCard != null)
            {
                if (_leaderCard.Skills[0] == 10001)
                {
                    if (cardEntry.Attribute != _leaderCard.Attribute)
                    {
                        SendTips("当前领袖卡不允许添加混合属性的卡牌!");
                        return;
                    }
                }
                else
                {
                    var temp = new HashSet<ElementAttribute>(_cardAttributes) { cardEntry.Attribute };
                    if (temp.Count(attribute => attribute != _leaderCard.Attribute) > 1)
                    {
                        SendTips($"当前领袖牌只允许有1种除<sprite name=\"{_leaderCard.SkillParams[0]}\">属性的卡!");
                        return;
                    }
                }
            }

            if (CardIds.Count(i => i == id) == 3)
            {
                SendTips("同名卡牌最多只能选择3张!");
                return;
            }
            
            if (cardEntry.Trigger != null)
            {
                if (_triggerCardCount == 8)
                {
                    SendTips("具有Trigger效果的卡牌最多只能选择8张!");
                    return;
                }

                _triggerCardCount++;
            }
            
            CardIds.Add(id);
            _cardAttributes.Add(cardEntry.Attribute);
        }

        private void OnRemoveCard(RemoveCardFromDeck e)
        {
            var index = CardIds.FindIndex(id => id == e.CardId);
            if (index != -1)
            {
                var cardEntry = DataManager.Instance.GetCard(CardIds[index]);
                if (cardEntry.CardType is CardType.Leader)
                {
                    _leaderCard = null;
                }

                CardIds.RemoveAt(index);
                if (CardIds.Select(id => DataManager.Instance.GetCard(id)).All(entry => entry.Attribute != cardEntry.Attribute))
                {
                    _cardAttributes.Remove(cardEntry.Attribute);
                }
            }
            else
            {
                SendTips("预设卡组中没有添加这张卡牌!");
            }
        }
        
        private void OnSave(SaveDeck e)
        {
            if (_leaderCard != null && CardIds.Count == 40)
            {
                PresetDeckNameInputUI.Create().SetTitle("请输入卡组名称").SetCallback(deckName =>
                {
                    var buildDeckEntry = new BuildDeckEntry(deckName, CardIds);
                    DeckEntries.Add(deckName, buildDeckEntry);
                    Save();
                    IsBuilding = false;
                });
            }
            else
            {
                SendTips("卡牌数量未达到40!");
            }
        }

        private void OnDelete(DeleteDeck e)
        {
            if (!DeckEntries.ContainsKey(e.Name))
            {
                return;
            }

            DeckEntries.Remove(e.Name);
            Save();
        }

        private void Save()
        {
            var json = JsonConvert.SerializeObject(DeckEntries);
            var filePath = Path.Combine(Application.persistentDataPath, "decks.json");
            File.WriteAllText(filePath, json);
        }

        private void SendTips(string tips)
        {
            MessageUI.Create().PositiveButton("OK").SetMessage(tips).SetOnClick((_, ui) =>
            {
                ui.CloseSelfByExt();
            });
        }

        private void SendTips(string tips, Action positive, Action negative)
        {
            MessageUI.Create().PositiveButton("OK").SetMessage(tips).SetOnClick((b, ui) =>
            {
                if (b is MessageUI.ButtonType.PositiveBtn)
                {
                    positive?.Invoke();
                }
                else if (b is MessageUI.ButtonType.NegativeBtn)
                {
                    negative?.Invoke();
                }

                ui.CloseSelfByExt();
            });
        }
    }
}