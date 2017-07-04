using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Core
{

	/// <summary>
	/// 化学方程式抽象
	/// @author DuckSoft
	/// </summary>
	public class Equation
	{
		/// <summary>
		/// 反应物列表 </summary>
		public IList<Pair<Formula, int?>> reactant = new List<Pair<Formula, int?>>();
		/// <summary>
		/// 生成物列表 </summary>
		public IList<Pair<Formula, int?>> product = new List<Pair<Formula, int?>>();

		public Equation()
		{

		}

		public override string ToString()
		{
			string strTemp = "";

			foreach (Pair<Formula, int?> pair in reactant)
			{
				if (pair.R1 != 1)
				{
					strTemp += pair.R1.ToString();
	//				strTemp += " ";
				}
				strTemp += pair.L1.RawString;
				strTemp += " + ";
			}
			strTemp = strTemp.Substring(0, strTemp.Length - 3);
			strTemp += " ---> ";

			foreach (Pair<Formula, int?> pair in product)
			{
				if (pair.R1 != 1)
				{
					strTemp += pair.R1.ToString();
	//				strTemp += " ";
				}
				strTemp += pair.L1.RawString;
				strTemp += " + ";
			}
			strTemp = strTemp.Substring(0, strTemp.Length - 3);
			return strTemp;
		}


		/// <summary>
		/// 以给定的含有有效格式的化学方程式字符串生成一个Equation对象。<br/>
		/// <para>可接受的格式示例如下：
		/// <li>2C + O2 = 2CO</li>
		/// <li>2C + O2 -> 2CO</li>
		/// <li>2C + O2 === 2CO</li>
		/// <li>2C + O2 ==> 2CO</li>
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Equation(String strEquation) throws Exception
		public Equation(string strEquation)
		{
			this.parseEquation(strEquation);
		}

		/// <summary>
		/// 分析方程式 </summary>
		/// <param name="strEquation"> 欲分析的方程式 </param>
		/// <exception cref="Exception"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void parseEquation(String strEquation) throws Exception
		public virtual void parseEquation(string strEquation)
		{
			// 不可重复分析
			Debug.Assert((this.reactant.Count == 0 && this.product.Count == 0));

			// 避免输入为空
			if (strEquation.Length == 0)
			{
				//TODO: 输入为空的处理
				throw new Exception("输入为空");
			}

			// 避免重复分析

			string partLeft = ""; // 反应物、生成物缓冲区
			string partRight = "";
			bool isRight = false; // 是否到了生成物标志
			bool bAuxFlag = false; // 辅助标志

			foreach (char i in strEquation.ToCharArray())
			{
				if (i == '=' || i == '-')
				{ // 出现 = 或 - 符号时判定为反应物结束
					isRight = true;
					if (bAuxFlag == true)
					{ // 若又出现一次分隔符，则判定为错误
						throw new Exception("出现多于一个的反应物-生成物分隔符");
					}
					continue;
				}
				else if (i == ' ' || i == '>')
				{ // 忽略掉 ---> 格式中的 > 字符以及空白字符
					continue;
				}
				else
				{
					if (isRight == true)
					{ // 若已到了生成物部分
						bAuxFlag = true; // 设定辅助标志
					}
				}

				if (isRight)
				{
					partRight += i.ToString();
				}
				else
				{
					partLeft += i.ToString();
				}
			}

			// 现在分离到列表里
			if (partLeft.Length == 0 || partRight.Length == 0)
			{
				throw new Exception("所输入的反应物或生成物为空");
			}

			bool isStarting = true; // 标志：是否是化学式的开头
			string strTempA = ""; // 系数存储
			string strTempB = ""; // 化学式存储

			foreach (char i in partLeft.ToCharArray())
			{
				if (isStarting == true)
				{ // 若为化学式的开头
					if (('0' <= i) && (i <= '9'))
					{ // 判定是否为数字
						strTempA += i.ToString(); // 若为数字系数则加入到系数暂存器
					}
					else
					{
						isStarting = false; // 若非则表示数字部分结束

						if (strTempA.Length == 0)
						{ // 处理没有系数的情况
							strTempA = "1"; // 给没有系数的项添加系数"1"
						}

						if (i == '+')
						{ // 防止开头就遇到"+"号的垃圾情况
							throw new Exception("列表开头遇到空白项");
						}

						strTempB += i.ToString(); // 将本个字符塞入化学式存储器
					}
				}
				else
				{ // 若非化学式的开头
					if (i == '+')
					{ // 若为"+"号
						if (strTempB.Length == 0 || strTempA.Length == 0)
						{ // 防止化学式或系数为空时加入列表
							throw new Exception("Equation::parseFormulaList: 列表内遇到空白项");
						}
						else
						{
							this.reactant.Add(new Pair<Formula, int?>(new Formula(strTempB),Convert.ToInt32(strTempA)));
							// 初始化状态 
							strTempA = "";
							strTempB = "";
							isStarting = true;
						}
					}
					else
					{ // 若非"+"号
						strTempB += i.ToString(); // 直接加入化学式缓冲
					}
				}
			}

			// 循环后处理
			if (!(strTempA.Length == 0 || strTempB.Length == 0))
			{
				this.reactant.Add(new Pair<Formula, int?>(new Formula(strTempB),Convert.ToInt32(strTempA)));
				// 初始化状态 
				strTempA = "";
				strTempB = "";
				isStarting = true;
			}

			strTempA = "";
			strTempB = "";
			isStarting = true;

			foreach (char i in partRight.ToCharArray())
			{
				if (isStarting == true)
				{ // 若为化学式的开头
					if (('0' <= i) && (i <= '9'))
					{ // 判定是否为数字
						strTempA += i.ToString(); // 若为数字系数则加入到系数暂存器
					}
					else
					{
						isStarting = false; // 若非则表示数字部分结束

						if (strTempA.Length == 0)
						{ // 处理没有系数的情况
							strTempA = "1"; // 给没有系数的项添加系数"1"
						}

						if (i == '+')
						{ // 防止开头就遇到"+"号的垃圾情况
							throw new Exception("列表开头遇到空白项");
						}

						strTempB += i.ToString(); // 将本个字符塞入化学式存储器
					}
				}
				else
				{ // 若非化学式的开头
					if (i == '+')
					{ // 若为"+"号
						if (strTempB.Length == 0 || strTempA.Length == 0)
						{ // 防止化学式或系数为空时加入列表
							throw new Exception("Equation::parseFormulaList: 列表内遇到空白项");
						}
						else
						{
							this.product.Add(new Pair<Formula, int?>(new Formula(strTempB),Convert.ToInt32(strTempA)));
							// 初始化状态 
							strTempA = "";
							strTempB = "";
							isStarting = true;
						}
					}
					else
					{ // 若非"+"号
						strTempB += i.ToString(); // 直接加入化学式缓冲
					}
				}
			}

			// 循环后处理
			if (!(strTempA.Length == 0 || strTempB.Length == 0))
			{
				this.product.Add(new Pair<Formula, int?>(new Formula(strTempB),Convert.ToInt32(strTempA)));
				// 初始化状态 
				strTempA = "";
				strTempB = "";
				isStarting = true;
			}
		}
	}

}