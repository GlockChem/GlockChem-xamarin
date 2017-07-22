using System.Diagnostics;

namespace Core
{
	/// <summary>
	/// AdvNum - 带误差计算的双精度类型
	/// @author DuckSoft
	/// @version 0.1
	/// </summary>
	public class AdvNum
	{
		// === 内部数据 ===
		/// <summary>
		/// AdvNum的内部数值。 </summary>
		/// <seealso cref= #numInner </seealso>
		/// <seealso cref= #numMin </seealso>
		/// <seealso cref= #numMax
		///  </seealso>
		internal double numInner;
		/// <summary>
		/// AdvNum的最大值。 </summary>
		/// <seealso cref= #numInner </seealso>
		/// <seealso cref= #numMin </seealso>
		/// <seealso cref= #numMax
		///  </seealso>
		internal double numMax;
		/// <summary>
		/// AdvNum的最小值。 </summary>
		/// <seealso cref= #numInner </seealso>
		/// <seealso cref= #numMin </seealso>
		/// <seealso cref= #numMax
		///  </seealso>
		internal double numMin;

		// === 类型转换 ===
		/// <summary>
		/// 将AdvNum对象转为String </summary>
		/// <returns> 所得String </returns>
		public override string ToString()
		{
			return "{" + this.numInner.ToString() + "; " + this.numMin.ToString() + ", " + this.numMax.ToString() + "}";
		}
		/// <summary>
		/// 将AdvNum对象转为double </summary>
		/// <returns> AdvNum对象的内部数值<seealso cref="#numInner"/> </returns>
		public virtual double toDouble()
		{
			return this.numInner;
		}
		// === 构造函数 ===
		/// <summary>
		/// 构造函数
		/// <para>该构造函数可以构造一个空的AdvNum。</para> </summary>
		/// <seealso cref= #AdvNum() </seealso>
		/// <seealso cref= #AdvNum(double) </seealso>
		/// <seealso cref= #AdvNum(double, double) </seealso>
		/// <seealso cref= #AdvNum(double, double, double)	  </seealso>
		public AdvNum() : base()
		{
		}
		/// <summary>
		/// 构造函数
		/// <para>该构造函数可以构造某一双精度值的AdvNum。</para> </summary>
		/// <param name="numIn"> 所需写入所得AdvNum中的双精度值 </param>
		/// <seealso cref= #AdvNum() </seealso>
		/// <seealso cref= #AdvNum(double) </seealso>
		/// <seealso cref= #AdvNum(double, double) </seealso>
		/// <seealso cref= #AdvNum(double, double, double)	  </seealso>
		public AdvNum(double numIn)
		{
			this.numInner = this.numMax = this.numMin = numIn;
		}
		/// <summary>
		/// 构造函数
		/// <para>该构造函数可以构造以某一双精度数为中心产生误差的AdvNum。</para> </summary>
		/// <param name="numCenter"> 所需写入的中心数值 </param>
		/// <param name="numError"> （<b>请给定正值</b>）为中心数值指定的误差数值 </param>
		/// <seealso cref= #AdvNum() </seealso>
		/// <seealso cref= #AdvNum(double) </seealso>
		/// <seealso cref= #AdvNum(double, double) </seealso>
		/// <seealso cref= #AdvNum(double, double, double)	  </seealso>
		public AdvNum(double numCenter, double numError)
		{
			// 误差值必须为非负数
			Debug.Assert(numError >= 0);

			this.numInner = numCenter;
			this.numMin = numCenter - numError;
			this.numMax = numCenter + numError;
		}
		/// <summary>
		/// 构造函数
		/// <para>该构造函数可以按用户的想法构建AdvNum。</para>
		/// <para><b>注意最大值务必要大于最小值！</b></para> </summary>
		/// <param name="numCenter"> 所需写入的中心数值 </param>
		/// <param name="numInMin"> 为中心数值指定的最小值 </param>
		/// <param name="numInMax"> 为中心数值指定的最大值 </param>
		/// <seealso cref= #AdvNum() </seealso>
		/// <seealso cref= #AdvNum(double) </seealso>
		/// <seealso cref= #AdvNum(double, double) </seealso>
		/// <seealso cref= #AdvNum(double, double, double)	  </seealso>
		public AdvNum(double numCenter, double numInMin, double numInMax)
		{
			// 务必最大值大于最小值
			Debug.Assert(numInMin <= numInMax);

			this.numInner = numCenter;
			this.numMin = numInMin;
			this.numMax = numInMax;
		}

		// === 基本运算函数 ===
		public virtual AdvNum add(AdvNum numIn)
		{
			double numInner = this.numInner + numIn.numInner;
			double numMin = this.numMin + numIn.numMin;
			double numMax = this.numMax + numIn.numMax;

			return new AdvNum(numInner, numMin, numMax);
		}
		public virtual AdvNum add(double numIn)
		{
			return add(new AdvNum(numIn));
		}
		public virtual AdvNum subtract(AdvNum numIn)
		{
			double numInner = this.numInner - numIn.numInner;
			double numMin = this.numMin - numIn.numMax;
			double numMax = this.numMax - numIn.numMin;

			return new AdvNum(numInner, numMin, numMax);
		}
		public virtual AdvNum subtract(double numIn)
		{
			return subtract(new AdvNum(numIn));
		}
		public virtual AdvNum multiply(AdvNum numIn)
		{
			double numInner = this.numInner * numIn.numInner;
			double numMin = this.numMin * numIn.numMin;
			double numMax = this.numMax * numIn.numMax;

			return new AdvNum(numInner, numMin, numMax);
		}
		public virtual AdvNum multiply(double numIn)
		{
			return multiply(new AdvNum(numIn));
		}
		public virtual AdvNum divide(AdvNum numIn)
		{
			double numInner = this.numInner / numIn.numInner;
			double numMin = this.numMin / numIn.numMax;
			double numMax = this.numMax / numIn.numMin;

			return new AdvNum(numInner, numMin, numMax);
		}
		public virtual AdvNum divide(double numIn)
		{
			return divide(new AdvNum(numIn));
		}

		// === 操作函数 ===
		public virtual AdvNum set(double numToSet)
		{
			this.numInner = this.numMin = this.numMax = numToSet;
			return this;
		}

		/// <summary>
		/// 获取AdvNum的中心化数
		/// <para>注意该方法仅获取中心化数，并不使得AdvNum中心化。<br>
		/// 若要令某个AdvNum中心化，请参见<seealso cref="#Centerize()"/>方法。</para> </summary>
		/// <seealso cref= #Centerize() </seealso>
		/// <returns> 中心化数——即该数最大值与最小值的平均数 </returns>
		public virtual double CenterizedNumber
		{
			get
			{
				return (this.numMin + this.numMax) / 2;
			}
		}

		/// <summary>
		/// 令AdvNum中心化
		/// <para>本方法可令AdvNum中心化，即使其内部数据变为该AdvNum最大值与最小值的平均数。<br>
		/// 若仅需获取某AdvNum的中心化数，请参见<seealso cref="#getCenterizedNumber()"/>方法。</para> </summary>
		/// <seealso cref= #getCenterizedNumber() </seealso>
		/// <returns> 所得AdvNum </returns>
		public virtual AdvNum Centerize()
		{
			this.numInner = this.CenterizedNumber;
			return this;
		}

		public virtual double NumMin
		{
			get
			{
				return this.numMin;
			}
		}

		public virtual double NumMax
		{
			get
			{
				return this.numMax;
			}
		}

		public virtual double ErrorWidth
		{
			get
			{
				return this.numMax - this.numMin;
			}
		}

		public virtual double ErrorMin
		{
			get
			{
				return this.numInner - this.numMin;
			}
		}

		public virtual double ErrorMax
		{
			get
			{
				return this.numMax - this.numInner;
			}
		}
	}

}