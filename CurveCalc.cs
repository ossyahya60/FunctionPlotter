
using System;
using System.Collections;
using info.lundin.Math;
using System.Text;

namespace DataPlotter
{
  public class Kurve
  {
	int nump,nroot,k;//number of points,number of ± operators, number of y-values
	double[,] curve; //Matrix containing all x- and y-values
	ArrayList tic=new ArrayList();//Arraylist containing the positions of the ticmarks
	ArrayList PosRoot=new ArrayList();//ArrayList containing the positions of the char ±  						 
	double x0,xn;	 //Start and end of the x-range
	string funkstr;  //formula of the function expressed as a string
	StringBuilder fnb=new StringBuilder();
	
	public Kurve(string fstr, double xa, double xe, int p)//Konstruktor
	{		
		nump=p;
		k=0;  //number of ± operators present in the formula
		x0=xa;// inicial x-value in the matrix
		xn=xe;// final x-value in the matrix
		funkstr=fstr;//function in string format
		StringBuilder fnb=new StringBuilder(funkstr);
						
		if (x0>xn)//inversion if erroneously range limits were confused
		{
		double swap=x0;
		x0=xn;
		xn=swap;
		}
		PosRoot=NumRoot(funkstr);//creates an ArrayList containing the positions
  						         //of the char ± in the formula
		nroot=RootCounter(funkstr);//determines the number of ± operators in formula
		k=2*nroot;               //number of y-values that must be stored in xyMatrix
								 //depending of the number of ± operators in the formula
		       
			if(nroot==0)		 		 //if no ± was present
			{
				curve=new double[nump,2];//only x- and y-values to be stored in curve
				xMatrix();				 //fills the x-values in the matrix (column 0)
				xyMatrix(0);             //fills the y-values in the matrix (column 1)
			}
			else 
			{	
				curve=new double[nump,1+k];//2*k y-values must be stored in the matrix
				xMatrix();				   //fills the x-values in the matrix (column 0)
				fnb=fnb.Replace("±","+");  //first, only positive signs are placed for ±									
				for(int j=0;j<=nroot-1;j++)
					{
					int z=Convert.ToInt32(PosRoot[j]);//Position of ± in funkstr
					fnb=fnb.Replace("+","-",z,1);	  //replaced successively by -
					funkstr=Convert.ToString(fnb);
					xyMatrix(j+1);                    //calculates the part of the matrix	
					}								  //corresponding to the actual formula
												
					for(int j=0;j<=nroot-1;j++)       //now the positions of - are changed
					{								  //successively for +
					int z=Convert.ToInt32(PosRoot[j]);	
					fnb=fnb.Replace("-","+",z,1);
					funkstr=Convert.ToString(fnb);	
					xyMatrix(1+nroot+j);			 //and the rest of the matrix is calculated			
				}		
			} 
	}
	
	//calculates all x-values to be stored in the 0-column of the matrix
	public double[,] xMatrix()
	{
		double increm=(xn-x0)/(nump-1);   //increment of x
		for(int i=0;i<=nump-1;i++)
		{						
			curve[i,0]=x0+(i)*increm;     //calculates the x-values in the matrix
		} 
		return curve;
	}
			
	//This method calculates the y-columns of the curve-matrix.
	//The columns of the curve matrix are: x, y1, y2, ... yk
	//beeing k the number of columns originated by the ± operators.
	public double[,] xyMatrix(int k) 
	{								
		int r;		
		if(k==0){r=1;}
		else{r=k;}
		ExpressionParser parser = new ExpressionParser();	
		for(int i=0;i<=nump-1;i++)
		{
		Hashtable h = new Hashtable();	
		h.Add("x",Convert.ToString(curve[i,0]));
	    curve[i,r]=parser.Parse( funkstr, h );//calculates yk
		}
		return curve;
	}
	
	//This method calculates a single y-value from x. If ± operators are present
	//in the formula then r is to be determined in the same way as in xyMatrix()
	//by successive replacement of the ± operator by - and +.
	public double yValue(double x,int r)
  	{
  		int j,z;
  		StringBuilder fnb=new StringBuilder(funkstr);
  		
  		if(nroot==0)				//no ± operator was present. Formula may be used
  		{							//directly
  			return CalcY(x,funkstr);
  		}
  		else						//otherwise the corresponding formula is formed
  		{  							//until it matches r.
  			fnb=fnb.Replace("±","+");   
  			for(j=0;j<=nroot-1;j++)
  			{ 		
  				z=Convert.ToInt32(PosRoot[j]);
  				fnb=fnb.Replace("+","-",z,1);
  				if(r==j+1){break;}
  			}
  			for(j=0;j<=nroot-1;j++)
  			{
  				if(r<=j+nroot){break;}  			
  				z=Convert.ToInt32(PosRoot[j]);
  				fnb=fnb.Replace("-","+",z,1);
  				if(r==j+nroot){break;}
  			}
  			funkstr=Convert.ToString(fnb);
  			return CalcY(x,funkstr);
  		}
  	}
  	
  	public double CalcY(double x,string fnb)
  	{
  		ExpressionParser parser=new ExpressionParser();
  		Hashtable h=new Hashtable();
  		h.Add("x",Convert.ToString(x));
  		return parser.Parse(fnb,h);  		
  	}
  	                    	
	//This method looks for the start of the curve which is not neccessarily at
	//the first point of the range. Also, the start of the curve may be at infinity;
	//in this case the next point is choosen. A value of the x-range is returned.
	//"column" cooresponds to the number of y-column in the xyMatrix
	public double CurveStart(int column)
	{
	  int j,t=column;
	  double firstX=0,foreX=0, z=0;
	  double increm=(xn-x0)/nump;
	  double y;
	  bool ynum, yinf;	 
		y=curve[0,t];	    	  //y at the start of the range	
		ynum=Double.IsNaN(y);     //ynum=false if this value is a number
		yinf=Double.IsInfinity(y);//yinf=true if this value is ±8
		
			if(ynum==false & yinf==false)//if the first value is a number and not infinit
			{
				return x0;				   //then no problem exists. Returns x0.
			}
			else						   //if not, now the first x-value giving
			{							   //a finit y-value is to be found in this column	
				for(j=1;j<=nump-1;j++)
				{
					y=curve[j,t];     		   //y-value at jth position of the matrix
					ynum=Double.IsNaN(y);      //is that a number? (false for any number)
					yinf=Double.IsInfinity(y); //is that infinit? (false if not infinit)
				
					if(ynum==false & yinf==false)//if this is a finit number:
					{
						firstX=curve[j,0];   //firstX is the first x-value that
						foreX=curve[j-1,0];  //gives a finit y-value! foreX is the 
						break;				 //previous x-value in the x-range
					}					
				}
			  
			}
		if(yinf==false)//The curve must start between foreX and firstX and the 
		{			   //starting point is found by iteration
			for(int k=0;k<50;k++)//starts an iteration for 20 times
			{
			z=foreX+(firstX-foreX)/2;//calculates an average between both values and	
			if(Double.IsNaN(yValue(z,t))==true){foreX=z;}//proves if an y-value exists
			else {firstX=z;}			
			}
		}
		return firstX;	//returns the aproximated  x-value of the start of the curve
	}
	
	//The following method works identical to CurveStart finding the last existing
	//x-value for a curve which is not necessarily at the end of the range
	public double CurveEnd(int column)
	{
		int j,t=column;
		double lastX=0,foreX=0, z=0;
		double increm=(xn-x0)/nump;
		double y=curve[nump-1,t];	    //y at the end of the range
		bool ynum=Double.IsNaN(y);  	//ynum=false if this value is a number
		bool yinf=Double.IsInfinity(y); //yinf=true if this value is infinit
		
		if(ynum==false & yinf==false)//if the last value is a number and not infinit
			{				
				return xn;				   //then no problem exists. Returns end of range.
			}
		else						   	   //if not the last x-value giving
			{							   //a reasonable number is to be found
				for(j=nump-1;j>=0;j--)
				{
					y=curve[j,t];              //yvalue at jth position of the matrix
					ynum=Double.IsNaN(y);      //is that a number?
					yinf=Double.IsInfinity(y); //is that infinit?
					
					if(ynum==false & yinf==false)//si es numero finito
					{
						lastX=curve[j,0];      //lastX is the last x-value that
						foreX=lastX+increm;    //gives a finit y-value! foreX is the
						break;				   //x-value right after in the x-range
					}					
				}		
			}
		
		if(yinf==false)
		{
			for(int k=0;k<50;k++)   //starts an iteration for 40 times
			{
			z=foreX-(foreX-lastX)/2;//calculates an average between both values and				
			if(Double.IsNaN(yValue(z,t))==false){lastX=z;}//proves if an y-value exists
			else {foreX=z;}			
			}
		}
			
		return lastX;
	}
	
	//writes the calculated matrix but is not used in the program
	public void WriteMatrix(int k,int nump)
	{
			for(int i=0;i<=nump-1;i++)
			{
			Console.Write("k= "+k+" x["+i+"]="+curve[i,0]+"  ");
				for(int j=1;j<=k;j++)
				{
				Console.Write(" y["+i+"]="+curve[i,j]+"  ");
				}
			Console.WriteLine();	
			}		
	}
	
	//detects the number of ± operators in the string
	public int RootCounter(string funkstr)
	{
		ArrayList rootlist=new ArrayList();//
		rootlist=NumRoot(funkstr);
		int num=Convert.ToInt32(rootlist.Count);
		return num;
	}
	
	//returns an ArrayList containing the positions of the ± operator
	//in the function-string
	public ArrayList NumRoot(string funkstr)
	{
		ArrayList PosRoot=new ArrayList();		
		for(int i=0;i<=funkstr.Length-1;i++)
		{	
			if(funkstr[i].ToString()=="±".ToString())
			{	
				PosRoot.Add(i);
			}	
		}
		return PosRoot;
	}
	
	//reads the y-value for the kth y-column at the position n in the curve matrix
	public double YValue(int n,int k)
	{
		return curve[n,k+1];		
	}
	
	//reads the x-value at the nth position in the curve matrix
	public double XValue(int n)
	{
		return curve[n,0];
	}
	
	//returns the number of powers of 10 covered by the x-range
	public int XPot()
	{
		double lox=xRange();
		int redwx=1;
		if(lox>1)
		{
			redwx=(int)Math.Floor((double)(Math.Abs((int)Math.Log10(lox))));
			return redwx;
		}
		else	
			return (int)(Math.Floor(Math.Log10((double)lox)));
	}
	
	//returns the number of powers of 10 covered by the y-range
	public int YPot()
	{
		double loy=Ymax()-Ymin();
		int redwy=1;		
		if(loy>1)
		{
			redwy=(int)Math.Floor(Math.Abs(Math.Log(loy)/Math.Log(10)));
		}
		return redwy;
	}
	
	//returns the highest y-value
	public double Ymax()
	{
		return Extrema(true);
	}
	
	//returns the lowest y-value
	public double Ymin()
	{
		return Extrema(false);
	}
	
	//returns extreme values and is used by Ymin or Ymax
	public double Extrema(bool task)
	{                              
		int i=0;
		int j=0;	
		int b=FirstNumber();//looks for the first real y-value in the matrix
		if(b!=-1)
		{
			double fxmax=curve[b,1];
			double fxmin=fxmax;
			double extrem,r;	
			if(nroot==0){r=0.5;}
			else r=nroot;
			for(i=b;i<=nump-1;i++)
			{
		  		for (j=1;j<=2*r;j++)
		  		{		  	
				 if(curve[i,j]>fxmax)
			 	    {
					fxmax=curve[i,j];
			  		}
			 	 if(curve[i,j]<fxmin)
			 	 	{
					fxmin=curve[i,j];
			 	 	}
		  		}
			}			
			if(task==true){extrem=fxmax;}
			else extrem=fxmin;
			return extrem;
		}		
		return Double.NaN;
	}
	
	//if the curve has undefined regions then the index of 
	//the first value that gives a real number is found.
	public int FirstNumber()
	{						
	int i,j,z=0;
	double r;
	int len=curve.GetLength(0);
	if(nroot==0){r=0.5;}
	else{r=nroot;}				         
	for(i=0;i<=len-1;i++)	
		for(j=1;j<=2*r;j++)
		{
			if(Double.IsNaN(curve[i,j])!=true)
			{	
			z=i;
			return z;
			}
		}		
		return -1;//if there was no real number over the whole range -1 is returned
	}
	
	//returns the x range
	public double xRange()
	{
		return xn-x0;
	}	

	//returns the value contained in the position pos of the ArrayList of ticmarcs
	public double Conv(ArrayList tic,int pos)
	{
		return Convert.ToDouble(tic[pos]);
	}
	
	//returns an ArrayList containing the ticmarks of the axis
	public ArrayList AxDiv(double xa, double xs) 
	{	
		if(xa==Double.NegativeInfinity){xa=-100000;}
		if(xs==Double.PositiveInfinity){xs= 100000;}			
		
		double diff=xs-xa;						//finds the range
		double num=diff/10;						//takes the 10th part
		int redw=(int)Math.Floor(Math.Log10(num));//finds the next minor
		double tlg=Math.Pow(10,redw);			//as a power of 10
		
		ArrayList tic=new ArrayList();
		if(diff/tlg<11){tlg=tlg;}				//only 10 divisions are allowed
		else if(diff/(2*tlg)<11){tlg=2*tlg;}	//this reduces their number by 2
		else if(diff/(5*tlg)<11){tlg=5*tlg;}	//this reduces by 5
		else {tlg=10*tlg;}						//and if necessary by 10
		
		if(xa/tlg==Math.Round(xa/tlg))			//only if the starting value of
		{										//the range is an entire this value
			tic.Add(xa);						//is taken as the first ticmark
		}
		else									//otherwise the range is amplified
		{										//to the left to the next ticmark
			tic.Add(Math.Floor(xa/tlg)*tlg);
		}
		
		while(Convert.ToDouble(tic[tic.Count-1])<xs)//now the rest of the ticmarcs
		{											//is added
			tic.Add(Convert.ToDouble(tic[tic.Count-1])+tlg);
		}
		return tic;
	}
	
	//shows the ticmarks. Not used in the program
	public void Teilung(ArrayList tic)
	{
		Console.WriteLine("Die Arrayliste sieht nun folgendemassen aus: ");
		Console.WriteLine("Die Liste hat total Glieder: "+tic.Count);
		
		for(int z=0;z<=tic.Count-1;z++){Console.Write(tic[z]+" ");}
		Console.WriteLine();
	}
		
	//This method converts numbers with too many ceros produced by rounding errors 
	//(p. e. 1.0000000008E-03) into shorter expressions and is used for ticmarks that are
	//too long to fit well in the label process of the axis.
    public double Rndhelp(double number)
	{
		string str1=Convert.ToString(number);//first, the number converts to a string
		string str="";
		string str3="";
		
		char ch2='E'; 
		char ch1=',';
		int i=0;
		int isK=str1.IndexOf(ch1);//Position of the coma. Gives -1 if there is no coma.
		int isE=str1.IndexOf(ch2);//Position of E if exponential notation.Gives -1 for normal notation.
		if(isE!=-1)				  //if the notation is exponential the string is split
		{
			str3=str1.Substring(isE,str1.Length-isE);//saves the exponential part
			str=str1.Substring(0,str1.Length-str3.Length);//Cuts off exponential part
		}
		else{str=str1;}           //otherwise it takes the unconverted number
		//now, only if the number is smaller than 1 and not zero!
		if(Math.Abs(Convert.ToDouble(str))<1&&Convert.ToDouble(str)!=0) 
		{																			
			char car='0';
			for(i=2;i<=str.Length-1;i++)
			{
				if(car!=str[i]&&ch1!=str[i]){break;}//detects zeros behind the comma
			}
			number=Convert.ToDouble(str)*Math.Pow(10,i-isK);
			number=Math.Round(number,5);//limits the maximum number of digits to 5
			str=Convert.ToString(number);
			
			if(isE!=-1)    //if there was no exponential part this reestablishes the
			{              // value of the number adding the corresponding exponential part
				str3=str.Substring(1,str3.Length-1);
				str=str+"E"+Convert.ToString(Convert.ToDouble(str3)-i+isK);			
			}
			else
			{	
				str=str+"E";     
				int expz=i-isK;
				str=str+"-"+expz;
			}	
		}
		else
		{
		number=Convert.ToDouble(str);
		number=Math.Round(number,4);
		str=Convert.ToString(number);
		
			if(isE!=-1)
			{
			str=str+str1.Substring(isE,str1.Length-isE);
			}
		}
		number=Convert.ToDouble(str);
		//if the value is insignificant in relation to the Range it is set to 0 to avoid
		//rounding errors for the 0-ticmark
		if(Math.Abs(number/xRange())<=1e-10){number=0;}
		return number;
	}
  }	
}
