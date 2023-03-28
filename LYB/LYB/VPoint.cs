using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using LYB.Properties;

namespace LYB
{
    public class VPoint
    {
        bool isPinned = false;
        bool fromBody = false;       
        Vec2 pos, old, vel, gravity, maxVel;
        int id;
        public float Mass;
        float radius, bounce, diameter, m, frict = .99f;
        float groundFriction = 0.7f;
        Color c;
        SolidBrush brush;
        public Graphics g;

        public bool FromBody
        {
            get { return fromBody; }
            set { fromBody = value; }
        }
        public float Diameter
        {
            get { return diameter; }
            set { diameter = value; }
        }
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public bool IsPinned
        {
            get { return isPinned; }
            set { isPinned = value; }
        }
        public float X
        {
            get { return pos.X; }
            set { pos.X = value; }
        }
        public float Y
        {
            get { return pos.Y; }
            set { pos.Y = value; }
        }
        public Vec2 Pos
        {
            get { return pos; }
            set { pos = value; }
        }
        public Vec2 Old
        {
            get { return old; }
            set { old = value; }
        }
        public float Radius
        {
            get { return radius; }
            set { radius = value; diameter = radius + radius; }
        }

        public VPoint(int x, int y)
        {
            this.id = -1;
            Init(x, y, 0, 0);
        }
        public VPoint(int x, int y, int id, bool Pinned)
        {
            this.id = id;
            isPinned = Pinned;
            Init(x, y, 0, 0);
        }

        public VPoint(int x, int y, int id)
        {
            this.id = id;
            Init(x, y, 0, 0);
        }
        public VPoint(int x, int y, float vx, float vy, int id,bool Pinned)
        {
            this.id = id;
            isPinned = Pinned;
            Init(x, y, vx, vy);           
        }

        public VPoint(int x, int y, float vx, float vy,int id)
        {
            this.id = id;
            Init(x, y, vx, vy);
        }

        private void Init(int x, int y, float vx, float vy)
        {
            pos         = new Vec2(x, y);
            old         = new Vec2(x, y);
            gravity     = new Vec2(0, .8);//
            vel         = new Vec2(vx, vy);
            maxVel      = new Vec2(20, 20);
            radius      = 10;
            diameter    = radius + radius;
            Mass        = 1f;
            bounce      = 1f;
            c = Color.Violet;
            brush = new SolidBrush(c);
            if (IsPinned)
            {
                Pin();
            }
        }

        public void Pin()
        {
            
            brush = new SolidBrush(Color.Gray);
            
            radius = 15;
            diameter = radius + radius;
            isPinned = true;
        }

        public void Update(int width, int height)
        {
            
            if (isPinned)
                return;//*/

            vel = (pos - old) *  frict;

            old = pos;
            pos += vel + gravity;

            if (vel.Length() > maxVel.Length())
            {
                vel = vel.Normalize() * maxVel.Length();
            }
        }

        public void Constraints(int width, int height)
        {
            if (pos.X > width - radius)     { pos.X = width - radius;   old.X = (pos.X + vel.X); }
            if (pos.X < radius)             { pos.X = radius;           old.X = (pos.X + vel.X); }
            if (pos.Y > height - radius)    { pos.Y = height - radius;  old.Y = (pos.Y + vel.Y); }
            if (pos.Y < radius)             { pos.Y = radius;           old.Y = (pos.Y + vel.Y); }
        }

        public void Render(Graphics g, int width, int height)
        {
            if (fromBody)
                return;

            Update(width, height);
            Constraints(width, height);

            g.FillEllipse(brush, pos.X - radius, pos.Y - radius, diameter, diameter);

            if(isPinned == true)
            {
                g.DrawImage(Resources._100_pts, pos.X - radius, pos.Y - radius, diameter, diameter);
            }

            if(isPinned!= true) 
            {
                g.DrawImage(Resources.Pinball, pos.X - radius, pos.Y - radius, diameter, diameter);
            }
            //g.DrawImage(Resources.Pinball, pos.X - radius, pos.Y - radius, diameter, diameter);
        }


        public override string ToString()
        {
            return "ID: "+id+" : "+pos.ToString();
        }

    }
}
