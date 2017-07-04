using System;

namespace Core
{
	// 并未完成
	// TODO:完善

	/// <summary>
	/// 矩阵类
	/// <para>为配合<seealso cref="EquationBalancer#balanceGaussian()"/>而写的一个矩阵的简易实现。</para>
	/// @author DuckSoft
	/// </summary>
	public sealed class Matrix
	{
		public int[][] matrix;

		/// <summary>
		///构造函数
		/// <para>构造一个新的矩阵类。<br>
		/// 注意：矩阵的行数和列数必须为正数！</para> </summary>
		/// <param name="lines"> 新矩阵的行数。 </param>
		/// <param name="cols"> 新矩阵的列数。 </param>
		public Matrix(int lines, int cols)
		{
			assert((lines > 0) && (cols > 0));
			// 初始化矩阵
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: matrix = new int[lines][cols];
			matrix = RectangularArrays.ReturnRectangularIntArray(lines, cols);
		}

        private void assert(bool v)
        {
            throw new NotImplementedException();
        }

        public Matrix(int[][] matArray)
		{
			this.matrix = matArray;
		}

		public int[][] toArray()
		{
			return this.matrix;
		}

		public override string ToString()
		{
			string strTemp = "";
			for (int ln = 0; ln < this.matrix.Length; ln++)
			{
				for (int col = 0; col < this.matrix[0].Length; col++)
				{
					strTemp += "[";
					strTemp += this.matrix[ln][col];
					strTemp += "]";
				}
				strTemp += "\n";
			}

			return strTemp;
		}

	}

}