using LAB1;
using System.Drawing;

namespace TestProject1
{
    [TestClass]
    public class CanvasTests
    {
        private Canvas canvas;

        [TestInitialize]
        public void Setup()
        {
            canvas = new Canvas(10, 10); 
        }
        [TestMethod]
        public void AddShape_AddsShapeToListAndDrawsOnGrid()
        {
            
            var circle = new Circle(5, 5, 3, '*');
            canvas.AddShape(circle);

            
            Assert.AreEqual(1, GetShapesCount(canvas)); 
            Assert.AreEqual('*', GetGrid(canvas)[5, 5]); 
        }

        [TestMethod]
        public void EraseShape_RemovesShapeFromListAndRedraws()
        {
            
            var circle = new Circle(5, 5, 3, '*');
            canvas.AddShape(circle);

            canvas.EraseShape(0);

            Assert.AreEqual(0, GetShapesCount(canvas)); 
            Assert.AreEqual('·', GetGrid(canvas)[5, 5]); 
        }

        [TestMethod]
        public void MoveShape_UpdatesShapePositionAndRedraws()
        {
            var circle = new Circle(5, 5, 3, '*');
            canvas.AddShape(circle);

            canvas.MoveShape(0, 9, 9);
            
            var updatedCircle = (Circle)GetShapes(canvas)[0];
            Assert.AreEqual(9, updatedCircle.X); 
            Assert.AreEqual(9, updatedCircle.Y);
            Assert.AreEqual('*', GetGrid(canvas)[7, 7]); 
            Assert.AreEqual('·', GetGrid(canvas)[5, 5]); 
        }

        [TestMethod]
        public void Clear_ResetsGridToBackground()
        {
            var circle = new Circle(5, 5, 3, '*');
            canvas.AddShape(circle);

            canvas.Clear();

            for (int i = 0; i < canvas.Height; i++)
            {
                for (int j = 0; j < canvas.Width; j++)
                {
                    Assert.AreEqual('·', GetGrid(canvas)[i, j]); 
                }
            }
        }

        [TestMethod]
        public void Redraw_DrawsAllShapes()
        {
            var circle = new Circle(5, 5, 3, '*');
            var rectangle = new LAB1.Rectangle(2, 2, 3, 3, '#');
            canvas.AddShape(circle);
            canvas.AddShape(rectangle);

            canvas.Redraw();

            Assert.AreEqual('*', GetGrid(canvas)[5, 5]); 
            Assert.AreEqual('#', GetGrid(canvas)[2, 2]); 
        }

        [TestMethod]
        public void GetShapesState_ReturnsSerializedShapes()
        {
            var circle = new Circle(5, 5, 3, '*');
            canvas.AddShape(circle);

            string state = canvas.GetShapesState();

            Assert.IsTrue(state.Contains("circle")); 
            Assert.IsTrue(state.Contains("\"Radius\": 3")); 
            Assert.IsTrue(state.Contains("\"X\": 5")); 
        }

        [TestMethod]
        public void SetShapesState_DeserializesAndSetsShapes()
        {
            string state = "[{\"$type\":\"circle\",\"Radius\":3,\"X\":5,\"Y\":5,\"Symbol\":\"*\"}]";

            canvas.SetShapesState(state);

            Assert.AreEqual(1, GetShapesCount(canvas));
            var loadedCircle = (Circle)GetShapes(canvas)[0];
            Assert.AreEqual(5, loadedCircle.X);
            Assert.AreEqual(5, loadedCircle.Y);
            Assert.AreEqual(3, loadedCircle.Radius);
            Assert.AreEqual('*', loadedCircle.Symbol);
            Assert.AreEqual('*', GetGrid(canvas)[5, 5]); 
        }

        
        private int GetShapesCount(Canvas canvas)
        {
            var field = typeof(Canvas).GetField("shapes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return ((List<Shape>)field.GetValue(canvas)).Count;
        }

        private List<Shape> GetShapes(Canvas canvas)
        {
            var field = typeof(Canvas).GetField("shapes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (List<Shape>)field.GetValue(canvas);
        }

        private char[,] GetGrid(Canvas canvas)
        {
            var field = typeof(Canvas).GetField("grid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (char[,])field.GetValue(canvas);
        }
    }
}

