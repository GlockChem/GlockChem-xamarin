using System;

namespace Core
{
	using AtomNameNotFoundException = Core.RMDatabase.AtomNameNotFoundException;

	/// <summary>
	/// 化学方程式计算类
	/// <para>用作化学方程式的简单比值计算</para>
	/// @author DuckSoft
	/// </summary>
	public sealed class EquationCalculator
	{
		/// <summary>
		/// 内部的化学方程式
		/// <para>由构造函数<seealso cref="#EquationCalculator(Equation)"/>为其赋值。</para> </summary>
		/// <seealso cref= #EquationCalculator(Equation) </seealso>
		internal Equation equInner;
		/// <summary>
		/// 用于计算分子量的数据库 </summary>
		internal RMDatabase dbRM = new RMDatabase();

		/// <summary>
		/// 构造函数
		/// <para>初始化一个化学方程式计算类。<br>
		/// 注意：该方程式<b>必须平衡</b>（即方程式左右两端必须<b>遵循质量守恒定律</b>）。<br/>
		/// 若方程式不平衡，程序会试图采用高斯消元法进行自动配平并用作计算。（该自动过程<b>会影响到传入的Equation对象</b>）<br>
		/// 若经过自动配平后仍无法平衡，程序会引发一个异常。</para> </summary>
		/// <param name="equToCalculate"> 用于计算的<seealso cref="Equation"/>对象 </param>
		/// <exception cref="Exception"> 引发的异常 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public EquationCalculator(Equation equToCalculate) throws Exception
		public EquationCalculator(Equation equToCalculate)
		{
			// 检查方程式是否已经平衡
			EquationBalancer balancer = new EquationBalancer(equToCalculate);

			if (balancer.checkBalance() == true)
			{
				// 已平衡则直接留用
				this.equInner = equToCalculate;
			}
			else
			{
				// 未平衡，尝试使用高斯消元法配平
				if (balancer.balanceGaussian() == false)
				{
					// 失败了
					throw new Exception("这方程式配不平，怎么算啦！吃屎啦您！");
				}
			}

		}

		/// <summary>
		/// 计算相应物质的质量
		/// <para></para> </summary>
		/// <param name="condition"> 提供的条件 </param>
		/// <param name="refItem"> 将被计算的物质 </param>
		/// <returns> 计算结果（质量） </returns>
		/// <exception cref="AtomNameNotFoundException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public AdvNum calcMass(EquationCondition condition, Pair<Formula, Nullable<int>> refItem) throws org.chembar.glockchem.core.RMDatabase.AtomNameNotFoundException
		public AdvNum calcMass(EquationCondition condition, Pair<Formula, int?> refItem)
		{
			return condition.getConditionMass(this.dbRM).divide(this.dbRM.queryMolarMass(condition.ConditionItem.L1)).divide((double)condition.ConditionItem.R1).multiply(this.dbRM.queryMolarMass(refItem.L1)).multiply((double)refItem.R1);
		}

		/// <summary>
		/// 计算相应物质的物质的量
		/// <para></para> </summary>
		/// <param name="condition"> 提供的条件 </param>
		/// <param name="refItem"> 将被计算的物质 </param>
		/// <returns> 计算结果（物质的量） </returns>
		/// <exception cref="AtomNameNotFoundException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public AdvNum calcMole(EquationCondition condition, Pair<Formula, Nullable<int>> refItem) throws org.chembar.glockchem.core.RMDatabase.AtomNameNotFoundException
		public AdvNum calcMole(EquationCondition condition, Pair<Formula, int?> refItem)
		{
			return condition.getConditionMole(this.dbRM).divide((double)condition.ConditionItem.R1).multiply((double)refItem.R1);
		}

		/// <summary>
		/// 计算条件（质量）
		/// <para>用于为<seealso cref="EquationCalculator"/>的计算提供条件。</para>
		/// @author DuckSoft
		/// </summary>
		public sealed class EquationConditionMass : EquationCondition
		{
			internal Pair<Formula, int?> refItem;
			internal AdvNum massInner;

			public EquationConditionMass(Pair<Formula, int?> refItem, AdvNum massInner)
			{
				this.massInner = massInner;
				this.refItem = refItem;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public AdvNum getConditionMass(RMDatabase rmDatabase) throws org.chembar.glockchem.core.RMDatabase.AtomNameNotFoundException
			public AdvNum getConditionMass(RMDatabase rmDatabase)
			{
				return this.massInner;
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public AdvNum getConditionMole(RMDatabase rmDatabase) throws org.chembar.glockchem.core.RMDatabase.AtomNameNotFoundException
			public AdvNum getConditionMole(RMDatabase rmDatabase)
			{
				return this.massInner.divide(rmDatabase.queryMolarMass(this.refItem.L1));
			}
			public Pair<Formula, int?> ConditionItem
			{
				get
				{
					return this.refItem;
				}
			}
		}

		/// <summary>
		/// 计算条件（物质的量）
		/// <para>用于为<seealso cref="EquationCalculator"/>的计算提供条件。</para>
		/// @author DuckSoft
		/// </summary>
		public sealed class EquationConditionMole : EquationCondition
		{
			internal Pair<Formula, int?> refItem;
			internal AdvNum moleInner;

			public EquationConditionMole(Pair<Formula, int?> refItem, AdvNum moleInner)
			{
				this.moleInner = moleInner;
				this.refItem = refItem;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public AdvNum getConditionMass(RMDatabase rmDatabase) throws org.chembar.glockchem.core.RMDatabase.AtomNameNotFoundException
			public AdvNum getConditionMass(RMDatabase rmDatabase)
			{
				return this.moleInner.multiply(rmDatabase.queryMolarMass(this.refItem.L1));
			}
			public AdvNum getConditionMole(RMDatabase rmDatabase)
			{
				return this.moleInner;
			}
			public Pair<Formula, int?> ConditionItem
			{
				get
				{
					return this.refItem;
				}
			}
		}

		/// <summary>
		/// 计算条件
		/// <para>用于为<seealso cref="EquationCalculator"/>的计算提供条件。</para>
		/// @author DuckSoft </summary>
		/// <seealso cref= EquationConditionMass </seealso>
		/// <seealso cref= EquationConditionMole </seealso>
		public interface EquationCondition
		{
			/// <summary>
			/// 获取条件中的质量 </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public AdvNum getConditionMass(RMDatabase rmDatabase) throws org.chembar.glockchem.core.RMDatabase.AtomNameNotFoundException;
			AdvNum getConditionMass(RMDatabase rmDatabase);
			/// <summary>
			/// 获取条件中的物质的量 </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public AdvNum getConditionMole(RMDatabase rmDatabase) throws org.chembar.glockchem.core.RMDatabase.AtomNameNotFoundException;
			AdvNum getConditionMole(RMDatabase rmDatabase);
			/// <summary>
			/// 获取条件中的整个表项 </summary>
			Pair<Formula, int?> ConditionItem {get;}
		}
	}

}