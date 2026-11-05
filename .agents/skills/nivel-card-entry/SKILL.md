---
name: nivel-card-entry
description: 根据卡牌图片或文字，在 Nivel Arena DB 项目中录入、修改或核对卡牌、技能和触发配置。适用于 cards.csv、triggers.csv、云端技能表及必要的枚举和中文映射维护；技能仅写云端，skills.csv 由用户统一生成。
---

# Nivel Arena 卡牌配置

把用户提供的卡面、技能截图和补充说明转换为项目配置。用户确认的中文名、属性、阵营和规则优先于图片里的机器翻译。仅要求检查或解释时保持只读；要求录卡时执行必要的配置更新。

## 数据来源与写入范围

以下路径相对项目根目录；开始时确认当前工作区含 `Assets/Resources/cards.csv`，读取最新文件，保留用户已有改动。

- **技能主表**：[Nivel Arena DB Skills](https://docs.google.com/spreadsheets/d/1A3aUkqP0SpAdcMjqN6A7gt95ChEFB96DX1s_wCUdFFs/edit)。使用当前可用的 Google Drive / Google Sheets 工具，遵循对应技能的读写规则。先读页签元数据，再读精确范围。
- **卡牌**：`Assets/Resources/cards.csv`，直接维护本地文件。
- **触发技能**：`Assets/Resources/triggers.csv`，直接维护本地文件。
- **本地技能快照**：`Assets/Resources/skills.csv`，可辅助查找历史模板，但可能落后于云端。**不要写入、同步或生成此文件，除非用户另行明确要求。** 用户会在全部录入完成后自行从 Excel 生成。
- 录卡可补充必要的枚举和 `ToChinese` 映射；不要顺带启用卡包、导入图片、修改 Addressables、重排既有 ID 或整理无关历史数据。

读取业务定义时，以这些文件为准：

- `Assets/Scripts/Runtime/Business/Data/` 下的 `Deck.cs`、`Rarity.cs`、`CardType.cs`、`ElementAttribute.cs`、`KeyType.cs`、`Affiliation.cs`。
- `Assets/Scripts/Runtime/Business/Data/Entry/CardEntry.cs`、`Map/CardEntryMap.cs`。
- `Assets/Scripts/Runtime/Business/Util/Extension.cs`：阵营/技能类型中文名与描述格式化。这里才是对应映射的位置，不是 `Framework/ZEvent/Extension/`。
- `Assets/Scripts/Runtime/Business/UI/CardDetail/SkillGroup.cs`、`CardDetailUI.cs`：图标参数与详情显示。

## 录入流程

1. 从图和用户说明提取官方卡号、中文名、类型、稀有度、属性、基础数值、阵营、签名情况、技能标签/阶段、效果正文及 Trigger。同一卡的基本面与觉醒面合并成一条记录。遇到影响规则含义的缺失信息再询问，不用机器翻译猜角色名或效果。
2. 读取最新 CSV 表头、目标卡包已有编号和相关枚举。根据记录主键识别新增还是更新；发现已存在的卡牌时核对内容，避免重复追加。
3. 先在对应云端技能页寻找可复用模板，同时检查双标签组合；Trigger 在本地触发表内查找。只为没有合适模板的效果分配新 ID。
4. 为描述生成有序参数，按运行时代码实际规则展开并核对全文。确认目标数量、阈值、比较方向、阶段、持续时间和授予效果均保留原意，再写入配置。
5. 云端写入后回读新增行和 Config!A1；本地用 CSV 解析器重新读取新增/更新记录。报告卡号、技能 ID、主要参数、必要代码改动和仍待用户确认的内容。

云端不可访问时继续识别卡面和准备候选，不将旧 `skills.csv` 当成最新编号来源，不绕过约定改写它。说明哪部分未保存；云端权限恢复后重新确认 ID，再完成依赖该 ID 的录入。

## 技能表与复用

- 每个技能页名称必须与 `KeyType` 枚举精确一致。页签名就是 `Key1`，列为 `Id,Key2,Description`。
- `Id` 与 `Description` 必填；只有双标签才填写 `Key2`，单标签留空。**空 `Key2` 与 `Key2=None` 不等价。**
- 技能卡（`CardType=Skill`）的效果描述放在 **`None` 页**。该页代表没有技能类型标签，仍然有完整效果；其他卡牌的无标签效果也可按卡面归入此页。
- 所有技能页共用一套 ID。新增时使用最新 Config!A1，并检查相关现有 ID，按全表实际最大值 + 1 分配；不要把本文件中的示例 ID 当作下一个 ID。
- Config!A1 的现有公式取各列出页签的最后一个非空 ID 再求最大值。保持每页 ID 升序、向末尾追加、Config 列表覆盖所有技能页；若发现公式与实际最大值不一致，先查明原因，不能直接沿用错误编号。
- 新页可能只有用户放入的占位 ID `1`。经确认只是占位时，把该行替换为正式技能，不保留无描述的占位记录。不要将任意低编号自动当作占位删除。
- 优先复用**标签组合与语义结构相同**的描述。数字、卡名、阵营、比较方向、属性/技能图标名和整段授予效果都可以参数化。
- 目标对象、支付条件、触发时机、持续时间不同不能硬套模板；尤其“此回合结束”“对手回合结束”“下个回合结束”不能互换。
- 新描述用 `{0}`、`{1}` 等拆出变化量；保留项目的简体中文表达及必要的 `<b>`、`<sprite name="...">` 标记。
- “获得【攻击者】……”中的攻击者属于被授予的效果，放在描述/参数内；不要因此把外层技能的 `Key2` 设为 `Attacker`，也不要独立占用卡牌的下一组技能。

## cards.csv 填写

始终按最新表头构造记录，保持列顺序和既有编码、引号与换行约定。当前为 28 列，完整表头及例子见 [字段与示例](references/fields-and-examples.md)。用 CSV 解析/序列化工具处理外层记录，不用逗号或物理行数直接拆整张 CSV。

- `Number`：卡包内部顺延 +1 的编号，保留现有三位格式，如 `001`。`Pack` 取 `Deck` 枚举。**记录唯一性由 Pack 与 Number 的组合确定**，代码 `CardEntry.Id` 当前格式为 `Pack-Number`。
- `Id2`：官方/卡面展示编号的覆盖值，不是关联旧记录的外键。若与记录编号不同就填写，否则留空；常见于 SB 卡包，但不限于旧卡重印。不得用它覆盖记录主键或直接定位另一条记录。
- `Rarity`、`CardType`、`Attribute` 必须使用枚举的实际拼写。地属性 / Land 对应 `Earth`；属性图标参数使用小写 `earth`。
- `Name` 使用用户确认的中文名。
- Leader：`Cost/Power/Hit` 全留空。Unit：填写三项。Skill、Item：填 `Cost`，`Power/Hit` 留空。空值用空单元格，不填 `-`。
- `Skill1–4`、`SkillParam1–4`、`IconTextParam1–4` 是四个对应组。从第 1 组开始连续填写，不跳组；无技能的组整组留空。读取器会压缩空技能 ID，却不压缩参数数组，跳组会导致错配。
- Leader 的誓约放第 1 组，觉醒随后，再填觉醒面的技能。当前组牌逻辑依赖第一技能；不能把它们当成任意展示顺序。
- `Trigger/TriggerParam` 独立于这四组；没有 Trigger 时两项都留空。
- `Affiliation` 为主要阵营。技能/道具默认 `None`，明确的卡面或特殊规则可例外，例如 `Covenant`、`Unique`；不能为统一格式抹掉已有例外。
- `Affiliation2` 为第二所属（第二阵营），位于 `Affiliation` 右侧。**所有类型的卡牌，如果卡面、提供的资料或用户说明没有明确提到两个所属，统一不填写 `Affiliation2`，保持空单元格。** 只有明确给出两个所属时才填写对应枚举；例如资料明确列出“百鬼夜行 / STRIKER”，才填写 `Hyakkiyako` + `Striker`。
- 不根据角色名、IP、卡牌类型或其他卡牌示例推断第二所属。没有提到第二所属时不必额外追问，也不填 `None`、`-` 或 `0`；`None` 会使详情页出现“第一阵营/-”。
- 缺少阵营枚举时，选择简短、明确且不冲突的名称，追加到 `Affiliation.cs` 末尾以保留既有数字值，并补 `Extension.cs` 的 `ToChinese`。已有 `Hyakkiyako → 百鬼夜行`、`Striker → STRIKER`、`Special → SPECIAL`。
- `HasSpecial` 表示签名版，`HasExtension` 表示特殊签名版，使用 `0/1`。按明确资料填写；未提供特殊版本信息时可按普通版 `0` 录入并说明假设，不从稀有度推断签名存在。现有显示逻辑要求 `HasExtension=1` 时 `HasSpecial=1`。

`Affiliation.Special` 是阵营值，与签名标志 `HasSpecial` 无关；技能 `EX` 与特殊签名标志 `HasExtension` 也无关。

不要硬编码旧列数。若用户再次改表头，核对 `CardEntry`、`CardEntryMap` 和使用方：当前 `Affiliation/Affiliation2/Id2` 的零基索引为 `23/24/27`；两个签名标志按列名读取。

## 描述参数与图标参数

`SkillParam` 和 `TriggerParam` 都由运行时按**半角逗号**拆分，再一次性调用 `string.Format`：

- 第一个值填 `{0}`，第二个值填 `{1}`，以此类推；重复占位符重用同一个值，不需要重复参数。
- 参数可以是字符串或富文本，正文标点用中文逗号。外层 CSV 的引号只能保护单元格，不能转义单元格内部作为参数分隔符的英文逗号。
- 有意义的空参数要保留，如 `exit,,1000`。无占位符的模板通常不填参数；有占位符时不可漏填。
- 参数整体为空时，代码直接返回原模板，不调用 Format，也不会因缺参数而抛出格式异常。因此还要主动检查未替换的占位符。
- 不在参数文本内再留下 `{0}` 一类占位符，运行时不会递归替换。检查参数不足、多余、顺序错配和展开后的残留花括号。

`IconTextParam` 控制技能标签精灵，不参与正文 Format：

- `Active`：主阶段 `main`，攻击阶段 `attack`。
- `LevelLink/Credits/Buff/Chain/EX`：按卡面标签填数值字符串，例如 EX：2 填 `2`，对应 `ex_2`。
- `Position`：`center` 或 `side`。其他标签是否需要值，以当前图集与卡面为准；无参数标签可留空。
- 双标签按 `Key1,Key2` 排列，第二标签才需要值时保留前置空项，如 `,main`；其他例子为 `9,main`、`center,main`。
- 标签数值与正文数值即使相同，也要分别填写。例如 EX：2 的标签参数为 `2`，正文“差值 2、战斗力 +2000”的参数为 `2,2000`。内文单独出现的 EX 使用 `<sprite name="ex">`。

图标可查 `Assets/Artworks/UI/Common/CardDetailUI/Textures/keyTypeGroup*.png`，精确名称以 `Assets/TextMesh Pro/Resources/Fonts & Materials/keyTypeGroup*.asset` 为准。新增 KeyType 时核对云端页、精灵和 `ToChinese` 三处支持；不要擅自重排用户已有枚举。

## triggers.csv 填写

列为 `Id,Description`。同一语义结构优先复用；新增时取触发表最新最大 ID + 1，与技能表是独立序列。描述和参数遵循上面的 Format 规则，不根据本技能文档中的历史编号分配新 ID。

## 完成前核对

- 卡牌主键唯一；同包新增编号正确；每行列数与当前表头一致，所有枚举与空值合法。
- 检查本次录入的 `Affiliation2`：有值必须能对应到资料或用户明确给出的双所属；未提到两个所属的卡牌统一留空。
- 技能引用以**当前云端表**核验，Trigger 以本地触发表核验。新云端技能尚未进入 `skills.csv` 是待用户导出的预期状态，不能据此报孤立引用或自行补写本地技能。
- 按程序规则展开新增卡的所有描述，逐项对照图；核对双标签位置、技能组连续性、Leader 空值、阵营中文映射和签名标志。
- 回读云端写入，确认正式 ID 唯一、占位记录正确替换、Config 下一编号正确。对新增的复杂或长描述检查可读性，保留现有表格结构和样式。
- 比较本次前后的相关数据，保留用户修正和无关记录；不要全量改写 CSV 来顺便清洗数据。
- 汇报实际保存位置和关键字段。区分数据/格式校验与真正的 Unity 运行验证；未运行时不要声称已通过运行验证。

导出工具的现有约束：`Assets/Scripts/Runtime/Editor/SkillExcelReader.cs` 固定读取 `Assets/Resources/Nivel Arena DB Skills.xlsx`；跳过第一张 Config 页，按页名生成 Key1，读取到存在但 Id 为空的行会停止该页。ID 单元格保持文本类型，避免中间空 Id 行。导出由用户执行，除非另有要求。
