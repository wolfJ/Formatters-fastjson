using System;
using DragonScale.Console;
using DragonScale.Portable;

namespace RoleGame
{
    public sealed class Treasure : MapCell
    {
        #region System defined privileges
        public static readonly Privilege Level1 = new Privilege<Treasure>("Level1");
        public static readonly Privilege Level2 = new Privilege<Treasure>("Level2");
        public static readonly Privilege Level3 = new Privilege<Treasure>("Level3");
        public static readonly Privilege Level4 = new Privilege<Treasure>("Level4");
        public static readonly Privilege Level5 = new Privilege<Treasure>("Level5");
        #endregion

        #region Fields
        private static readonly Privilege[] Levels = new Privilege[] { Level1, Level2, Level3, Level4, Level5 };
        private static Random random = new Random();
        #endregion

        #region Properties
        public Privilege Level { get; private set; }
        #endregion

        #region Ctor
        public Treasure()
        {
            Level = Levels[LevelValue = random.Next(0, 5)];
            LevelValue++;
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return "T";
        }

        //public override void OpenedBy(Player player)
        //{
        //    var keys = player.Keys;
        //    bool isOpened = false; ;
        //    foreach (var key in keys)
        //    {
        //        var role = key.Role;
        //        if (role != null && role.Contains(Level))
        //        {
        //            player.PickTreasure(this, key);
        //            isOpened = true;
        //            Output.Info("Hero " + player.Name + " get the " + LevelValue + " Level treasure.", ConsoleColor.Magenta);
        //            if (LevelValue == 5)
        //                Output.Info("Great!!!", ConsoleColor.Yellow);
        //            break;
        //        }
        //    }
        //    if (!isOpened)
        //        Output.Info("The treasure breaks.", ConsoleColor.DarkRed);
        //}
        #endregion
    }
}
