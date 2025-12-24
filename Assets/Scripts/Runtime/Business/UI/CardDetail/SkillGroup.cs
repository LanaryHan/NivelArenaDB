using System.Linq;
using System.Text;
using Runtime.Business.Data;
using Runtime.Business.Data.Entry;
using Runtime.Business.Util;
using TMPro;
using UnityEngine;

namespace Runtime.Business.UI.CardDetail
{
    public class SkillGroup : MonoBehaviour
    {
        public TMP_Text descTxt;

        public void Init(SkillEntry skillEntry, string descParam, string iconParam)
        {
            var builder = new StringBuilder();
            if (skillEntry.Key1 is not KeyType.None)
            {
                if (!string.IsNullOrEmpty(iconParam))
                {
                    if (skillEntry.Key2 != null)
                    {
                        var param = iconParam.Split(",").ToList();
                        if (param.Count == 1)
                        {
                            param.Add(string.Empty);
                        }

                        builder.Append(!string.IsNullOrEmpty(param[0])
                            ? $"<sprite name=\"{skillEntry.Key1.ToString().ToLower()}_{param[0]}\">"
                            : $"<sprite name=\"{skillEntry.Key1.ToString().ToLower()}\">");
                        builder.Append(!string.IsNullOrEmpty(param[1])
                            ? $"<sprite name=\"{skillEntry.Key2.Value.ToString().ToLower()}_{param[1]}\">"
                            : $"<sprite name=\"{skillEntry.Key2.Value.ToString().ToLower()}\">");
                    }
                    else
                    {
                        builder.Append($"<sprite name=\"{skillEntry.Key1.ToString().ToLower()}_{iconParam}\">");
                    }
                }
                else
                {
                    builder.Append($"<sprite name=\"{skillEntry.Key1.ToString().ToLower()}\">");
                    if (skillEntry.Key2 != null)
                    {
                        builder.Append($"<sprite name=\"{skillEntry.Key2.Value.ToString().ToLower()}\">");
                    }
                }
            }
            
            builder.Append(skillEntry.Description.ToDescription(descParam));
            descTxt.text = builder.ToString();
        }
    }
} 