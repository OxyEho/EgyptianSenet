﻿using System.Net.Mail;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace Game.GameLogic.Tests
{
    [TestFixture]
    public class TestToHouses
    {
        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void TestStepToHouseOfBeauty(int stepCount)
        {
            var game = new Game();
            var map = game.Map;
            var cone = map[1].State;
            cone.MakeStep(24, map);
            cone.MakeStep(stepCount, map);
            Assert.AreEqual(map[26].State, cone);
        }
        
        [Test]
        [TestCase(26, 5)]
        [TestCase(27, 4)]
        [TestCase(28, 3)]
        [TestCase(29, 2)]
        [TestCase(30, 1)]
        public void TestStepToOut(int position, int stepCount)
        {
            var game = new Game();
            var map = game.Map;
            var cone = map[1].State;
            cone.MakeStep(25, map);
            cone.MakeStep(position - 26, map);
            cone.MakeStep(stepCount, map);
            Assert.AreEqual(map[position].State, null);
        }
        
        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void TestStepToHouseOfRevivalFromHouseOfWater(int stepCount)
        {
            var game = new Game();
            var map = game.Map;
            var cone = map[1].State;
            cone.MakeStep(25, map);
            cone.MakeStep(1, map);
            cone.MakeStep(stepCount, map);
            Assert.AreEqual(map[15].State, cone);
        }
        
        [Test]
        public void TestStayInHouseOfWater()
        {
            var game = new Game();
            var map = game.Map;
            var cone = map[1].State;
            cone.MakeStep(25, map);
            cone.MakeStep(1, map);
            cone.MakeStep(5, map);
            Assert.AreEqual(map[27].State, cone);
        }
        
        [Test]
        [TestCase(26, 27)]
        [TestCase(26, 28)]
        [TestCase(26, 29)]
        public void TestCutInHouses(int positionCone, int positionCoil)
        {
            var game = new Game();
            var map = game.Map;
            var cone = map[1].State;
            var coil = map[2].State;
            coil.MakeStep(24, map);
            coil.MakeStep(positionCoil - 26, map);
            cone.MakeStep(25, map);
            cone.MakeStep(positionCone - 26, map);
            cone.MakeStep(positionCoil - positionCone, map);
            Assert.AreEqual(map[15].State, coil);
            Assert.AreEqual(map[positionCoil].State, cone);
        }

        [Test]
        [TestCase(28, 1)]
        [TestCase(29, 1)]
        [TestCase(30, 2)]
        public void TestStayInHouses(int positionCone, int stepCount)
        {
            var game = new Game();
            var map = game.Map;
            var cone = map[1].State;
            cone.MakeStep(25, map);
            cone.MakeStep(positionCone - 26, map);
            cone.MakeStep(stepCount, map);
            Assert.AreEqual(map[positionCone].State, cone);
        }
    }
}