namespace Core
{
	/// <summary>
	/// Pair
	/// @author DuckSoft
	/// </summary>
	/// @param <L> Pair左侧类型 </param>
	/// @param <R> Pair右侧类型
	///  </param>
	public class Pair<L, R>
	{
		private L l1;
		private R r1;
		public override string ToString()
		{
			return "( " + l1.ToString() + "," + r1.ToString() + ") ";
		}
		public Pair(L l, R r)
		{
			this.l1 = l;
			this.r1 = r;
		}
		public L L1
		{
			get
			{
				return l1;
			}
			set
			{
				this.l1 = value;
			}
		}
		public virtual R R1
		{
			get
			{
				return r1;
			}
			set
			{
				this.r1 = value;
			}
		}
	}
}