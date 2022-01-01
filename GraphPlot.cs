using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace DataPlotter
{
	public class GraphPlot : System.Windows.Forms.Form
	{
		private DataPlotter.Plotter pl;
		/// <summary>
		double xstart,xend;//first and last x
		int numpoints;     //number of points in the curvev
		double[,] xywert;  //matrix for x and y-values
		string fstr;       //formula string
		Kurve cl;          //Object Kurve contains methods for calculation
		private System.ComponentModel.Container components = null;
		/// </summary>
	   
		public GraphPlot(string f, double x0,double xe,int nump)
		{	
			xstart=x0;
			xend=xe;
			numpoints=nump;
			fstr=f;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{			
		 	this.cl = new Kurve(fstr,xstart,xend,numpoints);
			this.pl = new DataPlotter.Plotter(cl);
			this.SuspendLayout();	
			
			// pl
			// 
			this.pl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
						| System.Windows.Forms.AnchorStyles.Left) 
						| System.Windows.Forms.AnchorStyles.Right)));
			
			this.pl.BorderBottom = 50;
			this.pl.BorderLeft = 60;
			this.pl.BorderRight = 10;
			this.pl.BorderTop = 10;
			this.pl.ColorAxis = System.Drawing.Color.Black;
			this.pl.ColorBg = System.Drawing.Color.White;
			this.pl.ColorDraw = System.Drawing.Color.DarkRed;
			this.pl.ColorGrid = System.Drawing.Color.LightGray;
			this.pl.DrawMode = DataPlotter.Plotter.DrawModeType.Line;
			this.pl.FontAxis = new System.Drawing.Font("Arial", 8F);
			this.pl.Location = new System.Drawing.Point(0, 0);
			this.pl.Name = "pl";
			this.pl.PenWidth = 2;
			this.pl.Size = new System.Drawing.Size(955,705);//defines height and width of grid
			this.pl.TabIndex = 0;	
			
			//ArrayList containing ticmarks of x-axis:
			ArrayList ticmarkx=cl.AxDiv(xstart,xend);
			
			//defines distance between ticmarks of x-axis:
			this.pl.XGrid=cl.Conv(ticmarkx,1)-cl.Conv(ticmarkx,0);
			
			//ArrayList containing ticmarks of y-axis:
			ArrayList ticmarky=cl.AxDiv(cl.Ymin(),cl.Ymax());			
			
			try
			{
			this.pl.YGrid = (cl.Conv(ticmarky,1)-cl.Conv(ticmarky,0));
			this.pl.XRangeStart=cl.Conv(ticmarkx,0);
			this.pl.XRangeEnd=cl.Conv(ticmarkx,ticmarkx.Count-1);	
			this.pl.YRangeEnd = cl.Conv(ticmarky,ticmarky.Count-1);
			this.pl.YRangeStart =cl.Conv(ticmarky,0);			
			this.pl.Fstr=fstr;
			
			xywert=cl.xyMatrix(2*cl.RootCounter(fstr));
			
			pl.XData=XVektor();			
			pl.YData=YVektor();
			
			// GraphPlot
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.ClientSize = new System.Drawing.Size(960, 705);//Window size
		
			this.Controls.Add(this.pl);
			this.Name = "GraphPlot";
			this.Text = " y = "+fstr;//shows function on title board
			this.ResumeLayout(true);
			}

			catch(Exception e)
			{
				Console.WriteLine("Error: "+e.Message);
			}
				
		}
		#endregion


		private double[] XVektor()//Matrix representing x-values
		{		
			double[] x=new double[numpoints];
			for(int i=0;i<numpoints;i++)
			{
				x[i]=xywert[i,0];
			} 
			return x;
		}
		
		private double[,] YVektor()//Matrix representing y-values
		{	
			double r;
			int nroot=cl.RootCounter(fstr);
			if(nroot==0){r=0.5;}
			else {r=nroot;}
			double[,] y=new double[numpoints,Convert.ToInt32(2*r)];
										
		 		for(int i=0;i<numpoints;i++)				{				
					for(int j=0;j<=2*r-1;j++)
  					{
					y[i,j]=xywert[i,j+1];					
					}
				}			
			return y;
		}
	}
}

