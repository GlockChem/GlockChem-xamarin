using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Core
{

	/// <summary>
	/// 化学式抽象
	/// @author DuckSoft
	/// @version 0.4 Stable
	/// </summary>
	public class Formula
	{

		/// <summary>
		/// 化学式原始字符串
		/// <para>包含该Formula类初始化时传入的表示该化学式的String。</para> </summary>
		/// <seealso cref= #Formula(String) </seealso>
		internal string strRaw;
		/// <summary>
		/// 化学式原子清单
		/// <para>包含从Formula初始化时传入的String中分析得到的该化学式含有的所有原子及其数目的配对的列表。</para> </summary>
		/// <seealso cref= #Formula(String) </seealso>
		public IDictionary<string, int?> mapAtomList = new Dictionary<string, int?>();

		/// <summary>
		/// 获取化学式原始字符串
		/// <para>获取包含该Formula类初始化时传入的表示该化学式的String。</para> </summary>
		/// <seealso cref= #strRaw </seealso>
		/// <seealso cref= #Formula(String) </seealso>
		public virtual string RawString
		{
			get
			{
				return strRaw;
			}
		}

		public override string ToString()
		{
			return this.mapAtomList.ToString();
		}
		// 异常类
		public class InvalidFormulaException : Exception
		{
			private readonly Formula outerInstance;

			internal const long serialVersionUID = 2L;
			internal string formula;

			internal InvalidFormulaException(Formula outerInstance, string inStr)
			{
				this.outerInstance = outerInstance;
				this.formula = inStr;
			}

			public virtual string FormulaString
			{
				get
				{
					return this.formula;
				}
			}
		}

		/// <summary>
		/// 构造函数
		/// <para>从传入的化学式String中构建Formula。</para> </summary>
		/// <param name="inFormula"> 表示该化学式的String </param>
		/// <seealso cref= #strRaw </seealso>
		/// <exception cref="InvalidFormulaException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Formula(String inFormula) throws InvalidFormulaException
		public Formula(string inFormula)
		{
			this.strRaw = inFormula;
			this.parseFormula(inFormula, 1);
		}

		/// <summary>
		/// 列表合并算法
		/// <para>将传入的原子列表合并到该Formula的原子列表<seealso cref="#mapAtomList"/>中。</para> </summary>
		/// <param name="pairToInsert"> 将被合并的原子列表 </param>
		private void insertList(Pair<string, int?> pairToInsert)
		{
			int numTodo = 0;
			try
			{
				numTodo = this.mapAtomList[pairToInsert.L1].Value;
			}
			catch (Exception)
			{

			}
			finally
			{
				this.mapAtomList[pairToInsert.L1] = numTodo + pairToInsert.R1;
			}
		}

		/// <summary>
		/// 化学式分析算法
		/// <para>分析化学式String及其子String。</para> </summary>
		/// <param name="inFormula"> 要分析的String </param>
		/// <param name="numMultiplier"> 该段String原子数前的系数 </param>
		/// <exception cref="InvalidFormulaException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parseFormula(String inFormula, int numMultiplier) throws InvalidFormulaException
		private void parseFormula(string inFormula, int numMultiplier)
		{
			Match sm; // 正则匹配结果

            Regex e = new Regex(@"^([A-Z][a-z]*)(\d*)"),    // 原子匹配正则
                f = new Regex(@"\*(\d*)([^*]+)[\*]??"), // 段分隔符"*"匹配 
                g = new Regex(@"\(([^\*]*)\)(\d*)"),   // 括号匹配#1
                h = new Regex(@"\(([^\*\(]*)\)(\d*)");// 括号匹配#2
            while (inFormula.Length > 0)
			{
                Console.WriteLine("Here's the trace::" + inFormula);
				//sm = Regex.Matches(inFormula, e);
                // 找一坨原子 
                // sm[1]: 原子名称
                // sm[2]: 原子数量(有可能为空白)
                Console.WriteLine("Here's the trace::" + e.IsMatch(inFormula));
                if (e.IsMatch(inFormula))
				{ // 若成功提取出原子
					int tempNum;
                    sm = e.Match(inFormula);
					if (sm.Groups[2].ToString()=="")
					{ // 若没有下标
						tempNum = 1 * numMultiplier; // 默认下标为1
					}
					else
                    { // 有下标
                        Console.WriteLine("Here's the trace::" + sm.Groups[2].ToString());
                        tempNum = Convert.ToInt32(sm.Groups[2].ToString()) * numMultiplier;
                        
					}

					string tempStr = sm.Groups[1].ToString(); // 交给插入算法处理
					this.insertList(new Pair<string, int?>(tempStr, tempNum));
					inFormula = inFormula.Substring(sm.Groups[0].Length);
				}
				else if (inFormula[0] == '*')
				{ 
					if (f.IsMatch(inFormula))
					{
                        // 又来一段
                        sm = f.Match(inFormula); // 找段
                                                          // sm[1]: 段乘数
                                                          // sm[2]: 段内容
                        int tempNum;
						if (! sm.Groups[1].Success)
						{
							tempNum = 1;

						}
						else
						{
							tempNum = Convert.ToInt32(sm.Groups[1].ToString());
						}

						string strTemp = sm.Groups[2].ToString();
						inFormula = inFormula.Substring(sm.Groups[0].Length);
						this.parseFormula(strTemp, tempNum);
					}
					else
					{ // 空段的处理
						//TODO: 空段
					}
				}
				else if (inFormula[0] == '(')
				{
					// TODO: 括号的处理

					int intCounter = 0; // 第一次出现后半括号前 前半括号出现的次数

					foreach (char ch in inFormula.ToCharArray())
					{
						if (intCounter > 2)
						{
							break;
						}
						switch (ch)
						{
							case '(':
								intCounter++;
								break;
							case ')':
								break;
							case '*':
								break;
						}
					}

					if (intCounter == 1)
					{ // 仅仅出现了一次，说明在内层
						sm = h.Match(inFormula);

						if (sm.Groups[1].Success)
						{
							int tempNum;
							if (!sm.Groups[2].Success)
							{
								tempNum = 1 * numMultiplier;
							}
							else
							{
								tempNum = Convert.ToInt32(sm.Groups[2].ToString()) * numMultiplier;
							}

							string strTemp = sm.Groups[1].ToString();
							inFormula = inFormula.Substring(sm.Groups[0].Length);
							this.parseFormula(strTemp, tempNum);
						}
						else
						{
							//TODO: fucking
						}
					}
					else if (intCounter == 2)
					{ // 出现了不止一次，说明在外层

						sm = h.Match(inFormula);

						int tempNum;
						if (!sm.Groups[2].Success)
						{
							tempNum = 1 * numMultiplier;
						}
						else
						{
							tempNum = Convert.ToInt32(sm.Groups[2].ToString()) * numMultiplier;
						}

						string strTemp = sm.Groups[1].ToString();
						inFormula = inFormula.Substring(sm.Groups[0].Length);
						this.parseFormula(strTemp, tempNum);
					}
				}
				else
				{
					throw new InvalidFormulaException(this, "化学式中发现非法字符 - " + inFormula.Substring(0, 1));
				}
			}
		}
	}

}