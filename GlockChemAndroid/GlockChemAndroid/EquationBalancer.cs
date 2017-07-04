using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Core
{

	/// <summary>
	///方程式配平类
	/// <para>用作与化学方程式配平有关的操作。</para>
	/// @author DuckSoft
	/// </summary>
	public class EquationBalancer
	{
		internal Equation equInner;

		/// <summary>
		///构造函数 </summary>
		/// <param name="equation"> 欲进行操作的方程式 </param>
		public EquationBalancer(Equation equation)
		{
			this.equInner = equation;
		}

		/// <summary>
		/// 检查方程式是否平衡
		/// <para>遍历方程式内<seealso cref="Formula"/>内的原子个数表，以验证方程式是否平衡。<br>
		/// 若平衡，则返回{@code true}；若否，则返回{@code false}。</para> </summary>
		/// <returns> 方程式是否平衡 </returns>
		public virtual bool checkBalance()
		{
			IDictionary<string, int?> atomReactant = new Dictionary<string, int?>();
			IDictionary<string, int?> atomProduct = new Dictionary<string, int?>();

			foreach (Pair<Formula, int?> pair in this.equInner.reactant)
			{
				foreach (KeyValuePair<string, int?> entry in pair.L1.mapAtomList.SetOfKeyValuePairs())
				{
					int numTodo = 0;
					try
					{
						numTodo = atomReactant[entry.Key].Value;
					}
					catch (Exception)
					{

					}
					atomReactant[entry.Key] = entry.Value * pair.R1 + numTodo;
				}
			}

			foreach (Pair<Formula, int?> pair in this.equInner.product)
			{
				foreach (KeyValuePair<string, int?> entry in pair.L1.mapAtomList.SetOfKeyValuePairs())
				{
					int numTodo = 0;
					try
					{
						numTodo = atomProduct[entry.Key].Value;
					}
					catch (Exception)
					{

					}
					atomProduct[entry.Key] = entry.Value * pair.R1 + numTodo;
				}
			}

	//		System.out.println(atomReactant);
	//		System.out.println(atomProduct);

			foreach (KeyValuePair<string, int?> entry in atomReactant.SetOfKeyValuePairs())
			{
				if (atomProduct.ContainsKey(entry.Key))
				{
					if (atomProduct[entry.Key].Value == entry.Value)
					{
						continue;
					}
					else
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}



			return true;
		}

		/// <summary>
		/// 高斯消元法配平方程式
		/// <para>使用高斯消元法配平Equation。<br>
		/// 成功则返回true，失败则返回false。
		/// </para>
		/// </summary>
		/// <returns> 配平是否成功
		/// @author EAirPeter - 高斯消元算法
		/// @author LionNatsu - 重要思维提示
		/// @author DuckSoft - 周边整合 </returns>
		public virtual bool balanceGaussian()
		{
			// 获取反应物和生成物的数量
			int numReactant = this.equInner.reactant.Count;
			int numProduct = this.equInner.product.Count;

			// 尼玛空的方程式配个屁啊
			if ((numReactant == 0) || (numProduct == 0))
			{
				return false;
			}

			// 元素不守恒配个屁啊
			if (checkElementBalance() == false)
			{
				return false;
			}

			// 得到行数与列数，用作以后建立矩阵
			int cols = numReactant + numProduct;
			int lines = 0;

			// 原子类型表
			IDictionary<string, int?> mapAtom = new Dictionary<string, int?>();

			// 遍历反应物和生成物
			foreach (Pair<Formula, int?> pair in this.equInner.reactant)
			{
				foreach (KeyValuePair<string, int?> entry in pair.L1.mapAtomList.SetOfKeyValuePairs())
				{
					mapAtom[entry.Key] = 1;
				}
			}
			foreach (Pair<Formula, int?> pair in this.equInner.product)
			{
				foreach (KeyValuePair<string, int?> entry in pair.L1.mapAtomList.SetOfKeyValuePairs())
				{
					mapAtom[entry.Key] = 1;
				}
			}

			// 给原子类型编号，顺便得到总行数
			foreach (KeyValuePair<string, int?> entry in mapAtom.SetOfKeyValuePairs())
			{
				entry.Value = lines++;
			}

			// 建立矩阵对象
			Matrix mat = new Matrix(lines, cols);
			int[][] mtx = mat.matrix;

			// 填充矩阵
			int col = 0; // 当前的行号
			foreach (Pair<Formula, int?> pair in this.equInner.reactant)
			{
				foreach (KeyValuePair<string, int?> atomPair in pair.L1.mapAtomList.SetOfKeyValuePairs())
				{
					mtx[(int)mapAtom[atomPair.Key]][col] = (int)atomPair.Value;
				}
				col++;
			}
			foreach (Pair<Formula, int?> pair in this.equInner.product)
			{
				foreach (KeyValuePair<string, int?> atomPair in pair.L1.mapAtomList.SetOfKeyValuePairs())
				{
					mtx[(int)mapAtom[atomPair.Key]][col] = -(int)atomPair.Value;
				}
				col++;
			}

	//		System.out.println(mat);

			// 高斯消元
			// Author: EAirPeter
			{
				int[] pos = new int[lines];
				int rank = 0;
				for (int i = 0; rank < lines && i < cols; ++i)
				{
					if (mtx[rank][i] == 0)
					{
						int u = rank;
						while (u < lines && mtx[u][i] == 0)
						{
							++u;
						}
						if (u < lines)
						{
							lnSwap(mtx[rank], mtx[u], i);
						}
						else
						{
							continue;
						}
					}
					pos[rank] = i;
					lnSimplify(mtx[rank], i);
					for (int j = rank + 1; j < lines; ++j)
					{
						if (mtx[j][i] != 0)
						{
							lnSubstract(mtx[j], mtx[rank], i, i);
						}
					}
					++rank;
				}
				for (int i = 1; i < rank; ++i)
				{
					lnSimplify(mtx[i], pos[i]);
					for (int j = 0; j < i; ++j)
					{
						lnSubstract(mtx[j], mtx[i], pos[j], pos[i]);
					}
				}
				for (int i = 0; i < rank; ++i)
				{
					lnSimplify(mtx[i], pos[i]);
				}
			}
	//		System.out.println(mat);

			// 判断是否有解：
			// 除了最后一列，每一列都有且仅有一个正值
			{
				int numNonZero = 0;
				for (int j = 0; j < cols - 1; ++j)
				{
					for (int i = 0; i < lines; ++i)
					{
						if (mtx[i][j] != 0)
						{
							numNonZero++;
						}
					}
					if (numNonZero == 0 || numNonZero > 1)
					{
						return false;
					}
					else
					{
						numNonZero = 0;
						continue;
					}
				}
			}

			// 提取矩阵有效系数
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: int[][] numResult = new int[cols-1][2];
			int[][] numResult = RectangularArrays.ReturnRectangularIntArray(cols - 1, 2);
			for (int j = 0; j < cols - 1; ++j)
			{
				int i;
				for (i = 0; i < lines; ++i)
				{
					if (mtx[i][j] != 0)
					{
						numResult[i][0] = mtx[i][j];
						break;
					}
				}
				// 最后一列的数值是负数
				// 变回正数并保存
				numResult[i][1] = - mtx[i][cols - 1];
	//			System.out.print(String.valueOf(numResult[i][0]) + "---");
	//			System.out.println(numResult[i][1]);
			}


			// 得到左侧最小公倍数
			int numGCD = numResult[0][0];
			for (int i = 0; i < cols - 1; ++i)
			{ // 轮一遍
				numGCD = numGCD * numResult[i][0] / gcd(numResult[i][0],numGCD);
			}

			// 全部放缩到最小公倍数
			int scale = 1;
			for (int i = 0; i < cols - 1; ++i)
			{
				scale = numGCD / numResult[i][0];
				numResult[i][0] = numGCD;
				numResult[i][1] *= scale;

				// 检查：无解
				if (numResult[i][1] <= 0)
				{
					return false;
				}
			}

	//		// 用作显示numResult
	//		{
	//			String strTemp = new String();
	//			for (int ln=0; ln<numResult.length; ln++) {
	//				for (int col1=0; col1<numResult[0].length; col1++) {
	//					strTemp += "[";
	//					strTemp += numResult[ln][col1];
	//					strTemp += "]";
	//				}
	//				strTemp += "\n";
	//			}
	////			System.out.println(strTemp);
	//		}


			// 输出结果
			Console.Write("配平系数：");
			for (int i = 0; i < col - 1; ++i)
			{
				if (i < numReactant)
				{
					this.equInner.reactant[i].R1 = numResult[i][1];
				}
				else
				{
					this.equInner.product[i - numReactant].R1 = numResult[i][1];
				}
	//			System.out.print(numResult[i][1]);
	//			System.out.print(", ");
			}
			this.equInner.product[numProduct - 1].R1 = numResult[0][0];
	//		System.out.println(numResult[0][0]);

	//		System.out.println(this.equInner);
			return true;
		}

		private bool checkElementBalance()
		{
			// 反应物、生成物原子种类表
			IDictionary<string, bool?> map = new Dictionary<string, bool?>();
			foreach (Pair<Formula, int?> pair in this.equInner.reactant)
			{
				foreach (KeyValuePair<string, int?> atomPair in pair.L1.mapAtomList.SetOfKeyValuePairs())
				{
					map[atomPair.Key] = false;
				}
			}
			foreach (Pair<Formula, int?> pair in this.equInner.product)
			{
				foreach (KeyValuePair<string, int?> atomPair in pair.L1.mapAtomList.SetOfKeyValuePairs())
				{
					if (map.ContainsKey(atomPair.Key))
					{
						map[atomPair.Key] = true;
					}
					else
					{
						return false;
					}
				}
			}
			foreach (KeyValuePair<string, bool?> entry in map.SetOfKeyValuePairs())
			{
				if (entry.Value == false)
				{
					return false;
				}
			}

			return true;
		}

		private static void lnSubstract(int[] ln1, int[] ln2, int pos, int key)
		{
			Debug.Assert((ln1[pos] != 0) && (ln2[pos] != 0));

			int d = gcd(ln1[key], ln2[key]);
			int a1 = Math.Abs(ln2[key]) / d;
			int a2 = Math.Abs(ln1[key]) / d;
			if (ln1[key] * ln2[key] > 0)
			{
				for (int i = pos; i < ln1.Length; ++i)
				{
					ln1[i] = ln1[i] * a1 - ln2[i] * a2;
				}
			}
			else
			{
				for (int i = pos; i < ln1.Length; ++i)
				{
					ln1[i] = ln1[i] * a1 + ln2[i] * a2;
				}
			}
		}

		private static void lnSimplify(int[] ln, int pos)
		{
			Debug.Assert(ln[pos] != 0);

			int d = lnGcd(ln, pos);
			if (d > 1)
			{
				for (int i = pos; i < ln.Length; ++i)
				{
					ln[i] /= d;
				}
			}
			if (ln[pos] < 0)
			{
				for (int i = pos; i < ln.Length; ++i)
				{
					ln[i] = -ln[i];
				}
			}
		}

		private static int lnGcd(int[] ln, int pos)
		{
			int d = 0;
			for (int i = pos; d != 1 && i < ln.Length; ++i)
			{
				d = gcd(d, ln[i]);
			}
			return d;
		}

		private static void lnSwap(int[] ln1, int[] ln2, int pos)
		{
			for (int i = pos; i < ln1.Length; ++i)
			{
				int tmp = ln1[i];
				ln1[i] = ln2[i];
				ln2[i] = tmp;
			}
		}

		private static int gcd(int a, int b)
		{
			if (a < 0)
			{
				a = -a;
			}
			if (b < 0)
			{
				b = -b;
			}
			while (b != 0)
			{
				int c = a % b;
				a = b;
				b = c;
			}
			return a;
		}

	}

}