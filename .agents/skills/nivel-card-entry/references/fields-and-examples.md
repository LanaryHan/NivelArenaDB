# 字段与已确认示例

这些例子说明填写方式，ID 均为已录入的历史值。实际录卡先读当前文件与云端表，不重复添加示例，也不把示例 ID 当作下一个可用编号。

## 当前卡牌表头

```csv
Number,Pack,Rarity,CardType,Attribute,Name,Cost,Power,Hit,Skill1,SkillParam1,IconTextParam1,Skill2,SkillParam2,IconTextParam2,Skill3,SkillParam3,IconTextParam3,Skill4,SkillParam4,IconTextParam4,Trigger,TriggerParam,Affiliation,Affiliation2,HasSpecial,HasExtension,Id2
```

字段名为 `Rarity`、`Attribute`；技能枚举为 `Credits`、`EX`，不要按早期口述的拼写另造字段。

## 记录编号与展示编号

露姬-倒霉兔：

```text
Pack = SB01
Number = 008
Id2 = SB01-001
```

记录由 `(SB01, 008)` 唯一标识，当前代码生成 `SB01-008`；界面显示官方编号 `SB01-001`。不能将 Id2 当作指向另一条 cards 记录的外键。

## 若藻：Leader 与嵌套授予效果

基本数据：`ST12-001`，`若藻`，`Leader/L/Earth`，`Affiliation=Hyakkiyako`，`Affiliation2` 空；`HasSpecial=1`、`HasExtension=0`。`Cost/Power/Hit`、Trigger 和 Id2 均空。

```text
Skill1 = 10001
SkillParam1 = earth
IconTextParam1 = 空

Skill2 = 10002
SkillParam2 = 7
IconTextParam2 = 空

Skill3 = 10734
SkillParam3 = 1,4,以上,<sprite name="attacker">若此单位攻击并以战斗使命中值较低的敌方单位弃置，对对手造成该命中差值的伤害
IconTextParam3 = attack
```

`10001` 是地属性誓约，`10002` 是等级觉醒；第三组的云端页为 `Active`，`Key2` 空，模板为：

```text
选择场上{0}个命中为{1}或{2}的我方单位，该单位直到此回合结束为止，获得「{3}」。
```

Attacker 位于授予效果正文中。此卡只有三组技能，不因为内嵌 Attacker 新增第四组或第二标签。

## 知世：EX 与双阵营

基本数据：`ST12-002`，`知世`，`Unit/C/Earth`，`Cost=1`、`Power=2500`、`Hit=1`；当前卡表为 `Affiliation=Hyakkiyako`、`Affiliation2=Striker`。签名标志均为 `0`，Trigger 与 Id2 空。

此例提供的资料明确同时列出“百鬼夜行”和“STRIKER”，所以填写第二所属。若另一张卡只提到“百鬼夜行”，其 `Affiliation2` 必须留空，不能照此例补成 `Striker`；该留空规则适用于所有卡牌类型。

```text
Skill1 = 10735
SkillParam1 = 2,2000
IconTextParam1 = 2
```

云端页 `EX`，`Key2` 空；其正式记录替换了用户为避免 Config 公式报错而填的占位 ID `1`：

```text
我方的攻击阶段开始时，若于本次攻击阶段中发动的<sprite name="ex">数值总合为我方领袖等级或以下，且差值为{0}或以上则可以发动此效果。直到对手的回合结束为止，战斗力+{1}。
```

外标签是 `ex_2`，正文图标是 `ex`。两个位置的 `2` 各自填写，不能额外叠加或擅自把原文改写成“发动后的总值”。

## 双标签参数

- `Passive + LevelLink`，只有第二标签带等级：`IconTextParam=,10`。
- `LevelLink + Active`：`IconTextParam=9,main`。
- `Position + Active`：`IconTextParam=center,main`。

图标参数顺序由实际 Key1/Key2 决定，不按标签名字自行排序。

## Trigger 参数

例如模板 `20010` 使用三项参数，卡牌 `BT05-065` 的正确 `TriggerParam` 是 `1,1,道具`。多插一个 `以下` 会把卡片类型错误替换成“以下卡”，第四项被忽略。校验应检查最终展开的正文，不能只检查“参数数量不少于占位符数量”。
