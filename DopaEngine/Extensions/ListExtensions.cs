using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DopaEngine.Extensions
{
    public static class ListExtensions
    {
        public static void AddMultiple(this List<DEBaseObject> source, List<DEBaseObject> ListsAdd)
        {
            ListsAdd.ForEach((item) => source.Add(item));
        }
        public static void AddMultiple(this List<DEUIObject> source, List<DEUIObject> ListsAdd)
        {
            ListsAdd.ForEach((item) => source.Add(item));
        }
        public static void AddMultiple(this List<DEMapObject> source, List<DEMapObject> ListsAdd)
        {
            ListsAdd.ForEach((item) => source.Add(item));
        }
    }

    public static class Vector2Extensions
    {
        public static Vector2 GetRelativePosition(this Vector2 source)
            => source + DE.Get().VM.CurrentActivity.getCamera.Rect.Location.ToVector2();
        public static Vector2 Scale(this Vector2 source, float scale)
            => new Vector2(source.X * scale, source.Y * scale);
    }

}
