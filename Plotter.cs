
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace DataPlotter
{
	/// <summary>
	/// The Plotter class is a control to display 2D data
	/// in an X/Y coordinate system.
	
	/// </summary>
	public class Plotter : System.Windows.Forms.UserControl
	{
		private System.ComponentModel.Container components = null;
		private double xMarke,yMarke;
		private Kurve kurve;

		public Plotter(Kurve kurve)
		{
			this.kurve = kurve;
			InitializeComponent();
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// Plotter
			// 
			this.Name = "Plotter";
			this.Size = new System.Drawing.Size(272,216);
			this.Resize += new System.EventHandler(this.Plotter_Resize);
		}
		#endregion
		
		private Color colorDraw = Color.DarkBlue;	
		public Color  ColorDraw
		{
			get {return colorDraw;}
			set {colorDraw = value;}
		}

		private Color colorGrid = Color.LightGray;	
		public Color  ColorGrid
		{
			get {return colorGrid;}
			set {colorGrid = value;}
		}

		private Color colorBg = Color.White;
		public Color  ColorBg
		{
			get {return colorBg;}
			set {colorBg = value;}
		}

		private Color colorAxis = Color.Black;
		public Color  ColorAxis
		{
			get {return colorAxis;}
			set {colorAxis = value;}
		}

		private Font fontAxis = new Font("Arial", 8);
		public Font  FontAxis
		{
			get {return fontAxis;}
			set {fontAxis = value;}
		}

		private int penWidth = 2;
		public int  PenWidth
		{
			get {return penWidth;}
			set {penWidth = value;}
		}

		public enum DrawModeType { Line = 1};
		private DrawModeType drawMode = DrawModeType.Line;
		public DrawModeType  DrawMode
		{
			get {return drawMode;}
			set {drawMode = value;}
		}

		private int borderTop = 30;
		public int  BorderTop
		{
			get {return borderTop;}
			set {borderTop = value;}
		}

		private int borderLeft = 50;
		public int  BorderLeft
		{
			get {return borderLeft;}
			set {borderLeft = value;}
		}

		private int borderBottom = 50;
		public int  BorderBottom
		{
			get {return borderBottom;}
			set {borderBottom = value;}
		}

		private int borderRight = 5;
		public int  BorderRight
		{
			get {return borderRight;}
			set {borderRight = value;}
		}

		private double xRangeStart = 0;
		public double  XRangeStart
		{
			get {return xRangeStart;}
			set {xRangeStart = value;}
		}

		private double xRangeEnd = 100;
		public double  XRangeEnd
		{
			get {return xRangeEnd;}
			set {xRangeEnd = value;}
		}

		private double yRangeStart = 0;
		public double  YRangeStart
		{
			get {return yRangeStart;}
			set {yRangeStart = value;}
		}

		private double yRangeEnd = 100;
		public double  YRangeEnd
		{
			get {return yRangeEnd;}
			set {yRangeEnd = value;}
		}
				
		private double xGrid;
		public double  XGrid
		{
			get {return xGrid;}
			set {xGrid = value;}
		}
		
		private double yGrid = 10;
		public double  YGrid
		{
			get {return yGrid;}
			set {yGrid = value;}
		}
		
		private double[] xData;
		public double[] XData
		{
			get {return xData;}
			set {xData = value;}
		}
		
		private double[,] yData;
		public double[,] YData
		{
			get {return yData;}
			set {yData = value;}
		}
		
		private string fstr;
		public string Fstr
		{
			get {return fstr;}
			set {fstr = value;}
		}
		
		protected override void OnPaint(PaintEventArgs e) 
		{
			int x0, y0, x1, y1, w0, h0;
			int i, x, y, n, d ;
			string s;
			try	// overall exception handling to avoid crashes
			{
				// check properties
				if (xRangeEnd <= xRangeStart) return;
				if (yRangeEnd <= yRangeStart) return;
				if (xGrid <= 0) return;	

				// prepare the tools
				Graphics g = e.Graphics;
				e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
				g.FillRectangle(new SolidBrush(colorBg), ClientRectangle);
				Pen penDraw = new Pen(colorDraw, penWidth);
				Pen penGrid = new Pen(colorGrid, 1);
				Pen penAxis = new Pen(colorAxis, 1);
				SolidBrush brushAxis = new SolidBrush(colorAxis);

				// calculate coordinates, width and hight inside
				x0 = ClientRectangle.Left + borderLeft;//Abstand linker Fensterrand von 0-Bezugsposition
				y0 = ClientRectangle.Top + borderTop;  //Abstand oberer Fensterrand von 0-Bezugsposition
				w0 = ClientRectangle.Width - borderLeft - borderRight; //Fensterbreite
				h0 = ClientRectangle.Height - borderTop - borderBottom;//Fensterhöhe
				x1 = ClientRectangle.Right - borderRight;  //Abstand rechter Fensterrand von 0-Bezugsposition
				y1 = ClientRectangle.Bottom - borderBottom;//Abstand unterer Fensterrand von 0-Bezugsposition
				
				// draw Xgrid
				ArrayList ticx=kurve.AxDiv(xRangeStart,xRangeEnd);
				n=ticx.Count-1;
	
				if (n == 0) n = 1;			//if no division of the axis set n=1
				d = w0 / n;					//divides window width n times
				for (i = 0; i <= n-1; i++)	//and draws the gridlines
				{
					x = x0 + i * d;			//Counter
					g.DrawLine(penGrid, x, y0, x, y1);//draws gridline
					xMarke=Convert.ToDouble(ticx[i]);
					s = Convert.ToString(kurve.Rndhelp(xMarke));//string to label
					SizeF sf = g.MeasureString(s, fontAxis);//trims label
					g.DrawString(s, fontAxis, brushAxis, 
					x - sf.Width / 2, y1 + sf.Height / 2);  //prints label
				}		
				
				// draw Ygrid
				ArrayList ticy=kurve.AxDiv(yRangeStart,yRangeEnd);
				n=ticy.Count-1;
				
				if (n == 0) n = 1;
				d = h0 / n;
				for (i = 0; i <= n-1; i++)
				{
					y = y1 - i * d;
					g.DrawLine(penGrid, x0, y, x1, y);
					yMarke=Convert.ToDouble(ticy[i]);
					yMarke=kurve.Rndhelp(yMarke);
					string st=Convert.ToString(yMarke);
					if(yMarke>=99999){st=yMarke.ToString("E");}
					s=st;
					SizeF sf = g.MeasureString(s, fontAxis);
					g.DrawString(s, fontAxis, brushAxis, 
					5, y - sf.Height / 2);
				}
				
				// draw axis
				g.DrawRectangle(penAxis, x0, y0, w0, h0);

				// draw data if available
				
				if (xData == null || yData == null) return;//if no data exists
	
				// first convert the data arrays into points inside the axis rectangle
				Point[] pt = new Point[xData.Length];
				float r=0;
				int nroot=Convert.ToInt32(kurve.RootCounter(fstr));//finds number of curves to be drawn
					if(nroot==0){r=0.5F;}
					else{r=(float)nroot;}
			
				for(int run=0;run<=Convert.ToInt32(r*2)-1;run++)
				{
				//in the case that the curve does not exist at the beginning of the
				//range find now the index of the first existing point		
				double firstP=kurve.CurveStart(run+1);//gives 0 if curve starts at the begin of the range
				double lastP=kurve.CurveEnd(run+1);//do the same for the possible end of the curve
				
				//converts these points into coordinates									
				int firstX=Convert.ToInt32(x0+(firstP-xRangeStart)/(xRangeEnd-xRangeStart)*w0);				
				int lastX=Convert.ToInt32(x0+(lastP-xRangeStart)/(xRangeEnd-xRangeStart)*w0);											
				
				//Go on only if a defined curve exists! This means that firstX is different
				//from lastX.
				if(firstX!=lastX)
				  {	
				//Now find the corresponding y-Values and their coordinates						
				int firstY=Convert.ToInt32(y1-(kurve.yValue(firstP,run+1)-yRangeStart)/(yRangeEnd-yRangeStart)*h0);				
				int lastY=Convert.ToInt32(y1-(kurve.yValue(lastP,run+1)-yRangeStart)/(yRangeEnd-yRangeStart)*h0);	
				Point lastValidPt=new Point(firstX,firstY);
				Point EndPoint=new Point(lastX,lastY);
								
					for (i = 0; i <= pt.Length-1; i++)
					{							
						try	// catch invalid data points 
						{
						pt[i].X = Convert.ToInt32(x0 + 
								(xData[i] - xRangeStart) / (xRangeEnd - xRangeStart) * w0);
						pt[i].Y = Convert.ToInt32(y1 - 
								(yData[i,run] - yRangeStart) / (yRangeEnd - yRangeStart) * h0);						
						lastValidPt=new Point(pt[i].X,pt[i].Y);
						}
						catch(System.Exception )
						{ 
						pt[i] = lastValidPt;	// redraw last valid point on error
						}
					}
									
				// now draw the points
					for (i = 0; i < pt.Length-1; i++)
					{					
						if (i > 0)
						{
						g.DrawLine(penDraw, pt[i - 1], pt[i]);
						}
					}

					if((EndPoint!=pt[i]))
					{
						g.DrawLine(penDraw,EndPoint,pt[i]);
					}
				  }
				}
			}
			
			catch(System.Exception )
			{ }
		}

		private void Plotter_Resize(object sender, System.EventArgs e)
		{
			this.Refresh();
		} 
	}
}

