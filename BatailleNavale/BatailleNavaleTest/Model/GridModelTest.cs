using System;
using System.Numerics;
using System.Text;
using BatailleNavale.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BatailleNavaleTest.Model
{
    [TestClass]
    public class GridModelTest
    {
        private Random rnd = new Random();

        [TestMethod]
        public void CtorTest()
        {
            GridModel gridModel = new GridModel();
        }

        [TestMethod]
        public void AddBoatTest()
        {
            GridModel gridModel = new GridModel();
            byte[] buffer = new byte[8];
            rnd.NextBytes(buffer);

            gridModel.Boats.Add(new BoatModel(new Vector2(rnd.Next(0, GridModel.SizeX), rnd.Next(0, GridModel.SizeX)), rnd.Next(BoatModel.MaxSize), (BoatModel.Orientation)rnd.Next(0, 1)));
        }

        [TestMethod]
        public void AddHitTest()
        {
            GridModel gridModel = new GridModel();

            gridModel.Hits.Add(new Hit() { Position = new Vector2(rnd.Next(GridModel.SizeX), rnd.Next(GridModel.SizeY)) });
        }

        [TestMethod]
        public void BoatExistsTest()
        {
            GridModel gridModel = new GridModel();

            Vector2 boatPos1 = new Vector2(rnd.Next(), rnd.Next());
            Vector2 boatPos2 = new Vector2(rnd.Next(), rnd.Next());
            Vector2 boatPos3 = new Vector2(rnd.Next(), rnd.Next());

            BoatModel boatRnd = new BoatModel(boatPos1, rnd.Next(BoatModel.MaxSize), (BoatModel.Orientation)rnd.Next(0, 1), -1);
            BoatModel boatSize3Hor = new BoatModel(boatPos2, 3, BoatModel.Orientation.Horizontal, -1);
            BoatModel boatSize4Ver = new BoatModel(boatPos3, 4, BoatModel.Orientation.Vertical, -1);


            gridModel.Boats.AddRange(new BoatModel[] { boatRnd, boatSize3Hor, boatSize4Ver });

            // Basic test
            Assert.IsTrue(gridModel.BoatExists(boatPos1), "boatPos1");
            Assert.IsFalse(gridModel.HitExists(new Vector2(rnd.Next((int)boatPos1.X), rnd.Next())), "Random1");

            // Advanced horizontal test
            Assert.IsTrue(gridModel.BoatExists(boatPos2), "boatPos2");
            Assert.IsTrue(gridModel.BoatExists(new Vector2(boatPos2.X + 1, boatPos2.Y)), "Random2");

            // Advanced vertical test
            Assert.IsTrue(gridModel.BoatExists(boatPos3), "boatPos3");
            Assert.IsTrue(gridModel.BoatExists(new Vector2(boatPos3.X, boatPos3.Y + 3)), "Random3");
        }

        [TestMethod]
        public void HitExistsTest()
        {
            GridModel gridModel = new GridModel();

            Hit hit = new Hit
            {
                Position = new Vector2(rnd.Next(), rnd.Next())
            };

            gridModel.Hits.Add(hit);

            Assert.IsTrue(gridModel.HitExists(hit.Position));

            Assert.IsFalse(gridModel.HitExists(new Vector2(rnd.Next((int)hit.Position.X), rnd.Next())));
        }
    }
}