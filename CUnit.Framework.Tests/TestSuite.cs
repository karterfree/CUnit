using System;

namespace CUnit.Framework.Tests
{
	class Room {
	}

	class Kitchen : Room
	{
	}

	class Bedroom : Room
	{
	}

	class Guestroom : Bedroom
	{
	}

	class MasterBedroom : Bedroom
	{
	}


	[TestSuite]
	public class TestSuite: BaseTestSuite
	{
		[Test]
		public void Test() {
			object t = null;
			Assert.Should(t).BeEqual(string.Empty);

			string s = "";


			Assert.Should(t).NotBeEqual(string.Empty);
			bool b1 = true;
			Assert.Should(b1).BeTrue();

			int i1 = 8;
			Assert.Should(i1).BeNull();

			int i2 = 8;
			int? i3 = null;
			Assert.Should(i3).BeEqual(i2);

			object t2 = null;
			object t3 = null;
			Assert.Should(t2).BeEqual(t3);

			Assert.Should().Throw<OutOfMemoryException>(() => {
				
			});

			Assert.Should().Throw<OutOfMemoryException>(() => {
				throw new OutOfMemoryException();
			});

			Assert.Should().NotThrow(() => {

			});

			Assert.Should().NotThrow(() => {
				throw new OutOfMemoryException();
			});

			var room = new Room();
			var kitchen = new Kitchen();
			var bedroom = new Bedroom();
			var guestroom = new Guestroom();
			var masterBedroom = new MasterBedroom();
			Assert.Should(room).BeInstanceOf(typeof(Room));
			Assert.Should(room).BeInstanceOf(typeof(Kitchen));
			Assert.Should(kitchen).BeInstanceOf(typeof(Room));
			Assert.Should(kitchen).BeInstanceOf(typeof(Kitchen));
			Assert.Should(bedroom).BeInstanceOf(typeof(Room));
			Assert.Should(bedroom).BeInstanceOf(typeof(Bedroom));
			Assert.Should(bedroom).BeInstanceOf(typeof(Kitchen));
			Assert.Should(guestroom).BeInstanceOf(typeof(Room));
			Assert.Should(guestroom).BeInstanceOf(typeof(Bedroom));
			Assert.Should(guestroom).BeInstanceOf(typeof(Guestroom));
			Assert.Should(guestroom).BeInstanceOf(typeof(MasterBedroom));
			Assert.Should(masterBedroom).BeInstanceOf(typeof(Room));
			Assert.Should(masterBedroom).BeInstanceOf(typeof(Guestroom));

			Room room1 = null;
			Room room2 = new Room();
			Assert.Should(room1).BeNull();
			Assert.Should(room2).BeNull();
			Assert.Should(room1).NotBeNull();
			Assert.Should(room2).NotBeNull();
			Assert.Should(1).NotBeNull();
			
			Assert.Should("").NotBeNull();
		}
	}
}
