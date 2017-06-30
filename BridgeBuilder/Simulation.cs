using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BridgeBuilder
{
    class Simulation
    {
        public ConcurrentBag<Vertex> Vertices { get; private set; }

        public decimal Damping { get; set; } = 0.007M;
        public decimal Stiffness { get; set; } = 0.015M;
        public decimal GravitationStrength { get; set; } = 10000M;
        public decimal DraggingStrength { get; set; } = 5000M;
        public decimal DraggingDamping { get; set; } = 500M;
        public decimal GroundStrength { get; set; } = 50000M;
        public decimal GroundDamping { get; set; } = 50M;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public bool Gravitation = false;

        public Simulation(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            Vertices = new ConcurrentBag<Vertex>();
           
            var board = new Vertex[5, 5];
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    board[x, y] = new Vertex(this, x * 20 + 100, y * 20 + 100);
                    Vertices.Add(board[x, y]);
                }
            }
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    for (int dx = Math.Max(x-1, 0); dx <= Math.Min(x+1, board.GetLength(0)-1); dx++)
                    {
                        for (int dy = Math.Max(y - 1, 0); dy <= Math.Min(y + 1, board.GetLength(1)-1); dy++)
                        {
                            if(dx > x || dy > y)
                                board[x, y].AddEdge(board[dx, dy]);
                        }
                    }
                }
            }
            Vertices.First().Neighbours.First().Length -= 5;
        }

        public void Update(float dt)
        {
            foreach(var v in Vertices) v.Update(dt);
            foreach (var v in Vertices) v.Step();
        }

        public void AddVertex(float x, float y)
        {
            Vertices.Add(new Vertex(this, x, y));
        }
    }
}