using System.Collections;
using System.Collections.Generic;

namespace Core {
/** AdvNum - 带误差计算的双精度类型
 * @author DuckSoft
 * @version 0.1
 */
public class AdvNum {
	// === 内部数据 ===
	/** AdvNum的内部数值。
	 * @see #numInner
	 * @see #numMin
	 * @see #numMax
	 * */
	double numInner;
	/** AdvNum的最大值。
	 * @see #numInner
	 * @see #numMin
	 * @see #numMax
	 * */
	double numMax;
	/** AdvNum的最小值。
	 * @see #numInner
	 * @see #numMin
	 * @see #numMax
	 * */
	double numMin;
	
	// === 类型转换 ===
	/** 将AdvNum对象转为String
	 * @return 所得String
	 */
	public String toString() {
		return "{" + String.valueOf(this.numInner) + 
				"; " + String.valueOf(this.numMin) + 
				", " + String.valueOf(this.numMax) + "}";
	}
	/** 将AdvNum对象转为double
	 * @return AdvNum对象的内部数值{@link #numInner}
	 */
	public double toDouble() {
		return this.numInner;
	}
	// === 构造函数 ===
	/** 构造函数
	 * <p>该构造函数可以构造一个空的AdvNum。</p>
	 * @see #AdvNum()
	 * @see #AdvNum(double)
	 * @see #AdvNum(double, double)
	 * @see #AdvNum(double, double, double)	 */ 
	public AdvNum() {
		super();
	}
	/** 构造函数
	 * <p>该构造函数可以构造某一双精度值的AdvNum。</p>
	 * @param numIn 所需写入所得AdvNum中的双精度值
	 * @see #AdvNum()
	 * @see #AdvNum(double)
	 * @see #AdvNum(double, double)
	 * @see #AdvNum(double, double, double)	 */ 
	public AdvNum(double numIn) {
		this.numInner = this.numMax = this.numMin = numIn;
	}
	/** 构造函数
	 * <p>该构造函数可以构造以某一双精度数为中心产生误差的AdvNum。</p>
	 * @param numCenter 所需写入的中心数值
	 * @param numError （<b>请给定正值</b>）为中心数值指定的误差数值
	 * @see #AdvNum()
	 * @see #AdvNum(double)
	 * @see #AdvNum(double, double)
	 * @see #AdvNum(double, double, double)	 */ 
	public AdvNum(double numCenter, double numError) {
		// 误差值必须为非负数
		assert(numError >= 0);
		
		this.numInner = numCenter;
		this.numMin = numCenter - numError;
		this.numMax = numCenter + numError;
	}
	/** 构造函数
	 * <p>该构造函数可以按用户的想法构建AdvNum。</p>
	 * <p><b>注意最大值务必要大于最小值！</b></p>
	 * @param numCenter 所需写入的中心数值
	 * @param numInMin 为中心数值指定的最小值
	 * @param numInMax 为中心数值指定的最大值
	 * @see #AdvNum()
	 * @see #AdvNum(double)
	 * @see #AdvNum(double, double)
	 * @see #AdvNum(double, double, double)	 */ 
	public AdvNum(double numCenter, double numInMin, double numInMax) {
		// 务必最大值大于最小值
		assert(numInMin <= numInMax);
		
		this.numInner = numCenter;
		this.numMin = numInMin;
		this.numMax = numInMax;
	}
	
	// === 基本运算函数 ===
	public AdvNum add(AdvNum numIn) {
		double numInner = this.numInner + numIn.numInner;
		double numMin = this.numMin + numIn.numMin;
		double numMax = this.numMax + numIn.numMax;
		
		return new AdvNum(numInner, numMin, numMax);
	}
	public AdvNum add(double numIn) {
		return add(new AdvNum(numIn));
	}
	public AdvNum subtract(AdvNum numIn) {
		double numInner = this.numInner - numIn.numInner;
		double numMin = this.numMin - numIn.numMax;
		double numMax = this.numMax - numIn.numMin;
		
		return new AdvNum(numInner, numMin, numMax);
	}
	public AdvNum subtract(double numIn) {
		return subtract(new AdvNum(numIn));
	}
	public AdvNum multiply(AdvNum numIn) {
		double numInner = this.numInner * numIn.numInner;
		double numMin = this.numMin * numIn.numMin;
		double numMax = this.numMax * numIn.numMax;
		
		return new AdvNum(numInner, numMin, numMax);
	}
	public AdvNum multiply(double numIn) {
		return multiply(new AdvNum(numIn));
	}
	public AdvNum divide(AdvNum numIn) {
		double numInner = this.numInner / numIn.numInner;
		double numMin = this.numMin / numIn.numMax;
		double numMax = this.numMax / numIn.numMin;
		
		return new AdvNum(numInner, numMin, numMax);
	}
	public AdvNum divide(double numIn) {
		return divide(new AdvNum(numIn));
	}

	// === 操作函数 ===
	public AdvNum set(double numToSet) {
		this.numInner = this.numMin = this.numMax = numToSet;
		return this;
	}
	
	/** 获取AdvNum的中心化数
	 * <p>注意该方法仅获取中心化数，并不使得AdvNum中心化。<br>
	 * 若要令某个AdvNum中心化，请参见{@link #Centerize()}方法。</p>
	 * @see #Centerize()
	 * @return 中心化数——即该数最大值与最小值的平均数
	 */
	public double getCenterizedNumber() {
		return (this.numMin + this.numMax) / 2;
	}
	
	/** 令AdvNum中心化
	 * <p>本方法可令AdvNum中心化，即使其内部数据变为该AdvNum最大值与最小值的平均数。<br>
	 * 若仅需获取某AdvNum的中心化数，请参见{@link #getCenterizedNumber()}方法。</p>
	 * @see #getCenterizedNumber()
	 * @return 所得AdvNum
	 */
	public AdvNum Centerize() {
		this.numInner = this.getCenterizedNumber();
		return this;
	}
	
	public double getNumMin() {
		return this.numMin;
	}
	
	public double getNumMax() {
		return this.numMax;
	}
	
	public double getErrorWidth() {
		return this.numMax - this.numMin;
	}
	
	public double getErrorMin() {
		return this.numInner - this.numMin;
	}
	
	public double getErrorMax() {
		return this.numMax - this.numInner;
	}
}
/** 化学方程式抽象
 * @author DuckSoft
 */
public class Equation {
	/** 反应物列表*/
	public List<Pair<Formula,Integer>> reactant = new ArrayList<Pair<Formula,Integer>>();
	/** 生成物列表*/
	public List<Pair<Formula,Integer>> product = new ArrayList<Pair<Formula,Integer>>();
	
	public Equation() {
		
	}
	
	public String toString() {
		String strTemp = new String();
		
		for (Pair<Formula,Integer> pair : reactant) {
			if (pair.getR() != 1) {
				strTemp += String.valueOf(pair.getR());
//				strTemp += " ";
			}
			strTemp += pair.getL().getRawString();
			strTemp += " + ";
		}
		strTemp = strTemp.substring(0, strTemp.length()-3);
		strTemp += " ---> ";
		
		for (Pair<Formula,Integer> pair : product) {
			if (pair.getR() != 1) {
				strTemp += String.valueOf(pair.getR());
//				strTemp += " ";
			}
			strTemp += pair.getL().getRawString();
			strTemp += " + ";
		}
		strTemp = strTemp.substring(0, strTemp.length()-3);
		return strTemp;
	}
	
	
	/** 以给定的含有有效格式的化学方程式字符串生成一个Equation对象。<br/>
	 * <p>可接受的格式示例如下：
	 * <li>2C + O2 = 2CO</li>
	 * <li>2C + O2 -> 2CO</li>
	 * <li>2C + O2 === 2CO</li>
	 * <li>2C + O2 ==> 2CO</li>
	 * </p>
	*/
	public Equation(String strEquation) throws Exception {
		this.parseEquation(strEquation);
	}

	/** 分析方程式
	 * @param strEquation 欲分析的方程式
	 * @throws Exception
	 */
	public void parseEquation(String strEquation) throws Exception {
		// 不可重复分析
		assert((this.reactant.isEmpty() && this.product.isEmpty()));
		
		// 避免输入为空
		if (strEquation.isEmpty()) {
			//TODO: 输入为空的处理
			throw new Exception("输入为空");
		}
		
		// 避免重复分析
		
		String partLeft = "";		// 反应物、生成物缓冲区
		String partRight = "";
		boolean isRight = false;	// 是否到了生成物标志 
		boolean bAuxFlag = false;	// 辅助标志
		
		for (char i : strEquation.toCharArray()) {
			if (i == '=' || i == '-') {	// 出现 = 或 - 符号时判定为反应物结束 
				isRight = true;
				if (bAuxFlag == true) {	// 若又出现一次分隔符，则判定为错误 
					throw new Exception("出现多于一个的反应物-生成物分隔符");
				}
				continue;
			} else if (i == ' ' || i == '>') {	// 忽略掉 ---> 格式中的 > 字符以及空白字符
				continue;
			} else {
				if (isRight == true) {	// 若已到了生成物部分 
					bAuxFlag = true; 	// 设定辅助标志 
				}
			}
			
			if (isRight) {
				partRight += String.valueOf(i);
			} else {
				partLeft += String.valueOf(i);
			}
		}
		
		// 现在分离到列表里
		if (partLeft.isEmpty() || partRight.isEmpty()) {
			throw new Exception("所输入的反应物或生成物为空");
		}

		boolean isStarting = true;	// 标志：是否是化学式的开头
		String strTempA = "";			// 系数存储
		String strTempB = "";			// 化学式存储
		
		for (char i : partLeft.toCharArray()) {
			if (isStarting == true) {	// 若为化学式的开头 
				if (('0' <= i) && (i <= '9')) { // 判定是否为数字 
					strTempA += String.valueOf(i);				// 若为数字系数则加入到系数暂存器 
				} else {
					isStarting = false;	// 若非则表示数字部分结束
					
					if (strTempA.isEmpty()){	// 处理没有系数的情况 
						strTempA = "1";		// 给没有系数的项添加系数"1" 
					}
					
					if (i == '+') {	// 防止开头就遇到"+"号的垃圾情况 
						throw new Exception("列表开头遇到空白项");
					} 
					
					strTempB += String.valueOf(i);	// 将本个字符塞入化学式存储器 
				}
			} else {	// 若非化学式的开头 
				if (i == '+') {	// 若为"+"号 
					if (strTempB.isEmpty() || strTempA.isEmpty()) { // 防止化学式或系数为空时加入列表
						throw new Exception("Equation::parseFormulaList: 列表内遇到空白项");
					} else {
						this.reactant.add(new Pair<Formula,Integer>(new Formula(strTempB),new Integer(strTempA)));
						// 初始化状态 
						strTempA = "";
						strTempB = "";
						isStarting = true;
					}
				} else {		// 若非"+"号 
					strTempB += String.valueOf(i);	// 直接加入化学式缓冲 
				}
			}
		}
		
		// 循环后处理
		if (!(strTempA.isEmpty() || strTempB.isEmpty())) {
			this.reactant.add(new Pair<Formula,Integer>(new Formula(strTempB),new Integer(strTempA)));
			// 初始化状态 
			strTempA = "";
			strTempB = "";
			isStarting = true;
		}
		
		strTempA = "";
		strTempB = "";
		isStarting = true;
		
		for (char i : partRight.toCharArray()) {
			if (isStarting == true) {	// 若为化学式的开头 
				if (('0' <= i) && (i <= '9')) { // 判定是否为数字 
					strTempA += String.valueOf(i);				// 若为数字系数则加入到系数暂存器 
				} else {
					isStarting = false;	// 若非则表示数字部分结束
					
					if (strTempA.isEmpty()){	// 处理没有系数的情况 
						strTempA = "1";		// 给没有系数的项添加系数"1" 
					}
					
					if (i == '+') {	// 防止开头就遇到"+"号的垃圾情况 
						throw new Exception("列表开头遇到空白项");
					} 
					
					strTempB += String.valueOf(i);	// 将本个字符塞入化学式存储器 
				}
			} else {	// 若非化学式的开头 
				if (i == '+') {	// 若为"+"号 
					if (strTempB.isEmpty() || strTempA.isEmpty()) { // 防止化学式或系数为空时加入列表
						throw new Exception("Equation::parseFormulaList: 列表内遇到空白项");
					} else {
						this.product.add(new Pair<Formula,Integer>(new Formula(strTempB),new Integer(strTempA)));
						// 初始化状态 
						strTempA = "";
						strTempB = "";
						isStarting = true;
					}
				} else {		// 若非"+"号 
					strTempB += String.valueOf(i);	// 直接加入化学式缓冲 
				}
			}
		}
		
		// 循环后处理
		if (!(strTempA.isEmpty() || strTempB.isEmpty())) {
			this.product.add(new Pair<Formula,Integer>(new Formula(strTempB),new Integer(strTempA)));
			// 初始化状态 
			strTempA = "";
			strTempB = "";
			isStarting = true;
		}
	}
}

}