using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Shape  {

    public int area;
    public bool type = false;
    public List<Vector2> shape;

    public Shape()
    {
        shape = new List<Vector2>();
    }


    public Shape(bool _iS)
    {
        type = _iS;
    }

    public virtual void myNameIS()
    {

    }
    public class Rectangle : Shape
    {
        public int Height { get; set; }
        public int Width { get; set; }
        
        public Rectangle (int _W, int _H )
        {
            Width = _W;
            Height = _H;
            if (Width == Height) { type = true; }
        }

        public Rectangle() : base() {}
        public Rectangle(bool _iS) : base(_iS) { }

    }

    public class Triangle : Shape
    {
        public int a;
        public int b;
        public int c;


        public Triangle(int _a, int _b, int _c)
        {
            a = _a;
            b = _b;
            c = _c;
        }

        public Triangle() : base()
        {

        }
        public Triangle(bool _iS) : base(_iS) {  }

    }

    public class Diamond : Shape
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public Diamond(int _W, int _H)
        {
            Width = _W;
            Height = _H;
            if (Width == Height) { type = true; }

        }
        public Diamond(bool _iS) : base(_iS) { }
        public Diamond() : base()
        {

        }



    }

}
