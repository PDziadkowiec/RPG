using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Media;

namespace RPG
{
    public class Hero //bohater
    {
        public int HP;
        public int HPMax;
        public int MP;
        public int MPMax;
        public int Strength;
        public int Money;
        public int Magic;
        public bool Guard = false;
        public bool Armor = false;
        public bool Weapon = false;
        public int Experience = 0;
        public int potions = 0;
        public Hero(int H, int M, int S, int Mon, int Mag)
        {
            HP = H;
            MP = M;
            Strength = S;
            Money = Mon;
            Magic = Mag;
            HPMax = H;
            MPMax = M;
        }
        public void showstats()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("HP:" + HP + '/' + HPMax);
            showbars(HP, HPMax, ConsoleColor.Green, ConsoleColor.Red);
            Console.WriteLine("Mikstury: " + potions);
            Console.WriteLine("Sila:" + Strength);
            Console.WriteLine("pieniadze:" + Money);
            Console.WriteLine("doświadczenie: " + Experience);
        }
        public void showbars(int stat, int statmax, ConsoleColor full, ConsoleColor empty)
        {
            Console.Write(" ");
            for (int i = 1; i <= statmax; i++)
            {
                if (i <= stat)
                {
                    Console.BackgroundColor = full;
                }
                else Console.BackgroundColor = empty;
                Console.Write(" ");
            }
            Console.ResetColor();
            Console.WriteLine();
        }
    }
    class Map
    {
        Random encounter = new Random();
        static public int[,] EncounterID = new int[20, 20]; // tu można zmieniać rozmiar świata - tyle samo kolumn co rzędów bo program głupieje
        public bool[,] explored = new bool[EncounterID.GetLength(0), EncounterID.GetLength(1)];
        public bool[,] partyposition = new bool[EncounterID.GetLength(0), EncounterID.GetLength(1)];
        public bool[,] surroundings = new bool[EncounterID.GetLength(0), EncounterID.GetLength(1)];
        public bool[,] revisitable = new bool[EncounterID.GetLength(0), EncounterID.GetLength(1)];
        public bool[,] impassable = new bool[EncounterID.GetLength(0), EncounterID.GetLength(1)];
        public void Generate() //generowanie mapy świata
        {
            int position = encounter.Next(0, EncounterID.GetLength(0) - 1);
            for (int column = 0; column < EncounterID.GetLength(0); column++)
            {
                for (int row = 0; row < EncounterID.GetLength(1); row++)
                {
                    surroundings[row, column] = false;
                    partyposition[row, column] = false;
                    explored[row, column] = false;
                    impassable[row, column] = false;
                    EncounterID[column, row] = encounter.Next(0, 10); // modyfikacja wartości next wiąże się z dodaniem casów do switchy w refresh(2) i encounter.explore(1)
                }
            }
            EncounterID[encounter.Next(0, EncounterID.GetLength(0) - 1), encounter.Next(0, EncounterID.GetLength(1) - 1)] = 1001;
            partyposition[position, position] = true;
            explored[position, position] = true;
        }
        public void refresh() //odświeżanie mapy świata, odkryte pola, pozycja drużyny
        {
            Console.Clear();
            int checkrow = 0;
            for (int column = 0; column < EncounterID.GetLength(0); column++)
            {
                for (int row = 0; row < EncounterID.GetLength(1); row++)
                {
                    try
                    {
                        checksurroundings(column, row);
                    }
                    catch (System.IndexOutOfRangeException) { }
                    if (explored[column, row] == false)
                    {
                        if (surroundings[column, row] == true)
                        {
                            switch (EncounterID[column, row]) // pola odkryte niewyeksplorowane
                            {
                                case 0:
                                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                                    Console.Write("  ");
                                    Console.ResetColor();
                                    break;
                                case 1:
                                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                                    Console.ForegroundColor = ConsoleColor.DarkRed;
                                    Console.Write("XX");
                                    Console.ResetColor();
                                    break;
                                case 2:
                                    goto case 0;
                                case 3:
                                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    Console.Write("TT");
                                    Console.ResetColor();
                                    break;
                                case 4:
                                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.Write("~~");
                                    Console.ResetColor();
                                    impassable[column, row] = true;
                                    break;
                                case 5:
                                    goto case 0;
                                case 6:
                                    goto case 0;
                                case 7:
                                    goto case 0;
                                case 8:
                                    goto case 0;
                                case 9:
                                    goto case 0;
                                case 1001:
                                    Console.BackgroundColor = ConsoleColor.Red;
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.Write("!!");
                                    Console.ResetColor();
                                    break;
                            }
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.Gray;
                            Console.Write("  ");
                            Console.ResetColor();
                        }
                    }
                    if (explored[column, row] == true)
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        if (partyposition[column, row] == true)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("<>");
                        }
                        else
                        {
                            switch (EncounterID[column, row]) // pola wyeksplorowane
                            {
                                case 0:
                                    Console.BackgroundColor = ConsoleColor.Green;
                                    Console.Write("  ");
                                    break;
                                case 1:
                                    goto case 0;
                                case 2:
                                    Console.BackgroundColor = ConsoleColor.Green;
                                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                                    Console.Write("^^");
                                    break;
                                case 3:
                                    Console.BackgroundColor = ConsoleColor.Green;
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    Console.Write("TT");
                                    break;
                                case 4:
                                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.Write("~~");
                                    Console.ResetColor();
                                    impassable[column, row] = true;
                                    break;
                                case 5:
                                    goto case 0;
                                case 6:
                                    goto case 0;
                                case 7:
                                    goto case 0;
                                case 8:
                                    goto case 0;
                                case 9:
                                    goto case 0;
                                case 1001:
                                    goto case 0;
                            }
                        }
                        Console.ResetColor();
                    }
                    checkrow++;
                    if (checkrow == EncounterID.GetLength(0))
                    {
                        checkrow = 0;
                        Console.Write("||\n");
                    }
                }
            }
            for (int i = 0; i <= EncounterID.GetLength(0); i++)
            {
                Console.Write("==");
            }
            Console.WriteLine();
            Console.ResetColor();
        }
        public void movement(char side, ref Hero hero, ref Map world,SoundPlayer Overworld) //ruch postaci
        {
            int column = 0;
            int row = 0;
            foreach (bool x in partyposition)
            {
                if (x == true)
                {
                    partyposition[row, column] = false;
                    break;
                }
                column++;
                if (column > EncounterID.GetLength(0) - 1)
                {
                    column = 0;
                    row++;
                }
            }
            switch (side)
            {
                case 'w':
                    if (row == 0)
                    {
                        goto default;
                    }
                    row -= 1;
                    if (impassable[row, column] == true)
                    {
                        row += 1;
                    }
                    partyposition[row, column] = true;
                    break;
                case 'a':
                    if (column == 0)
                    {
                        goto default;
                    }
                    column -= 1;
                    if (impassable[row, column] == true)
                    {
                        column += 1;
                    }
                    partyposition[row, column] = true;
                    break;
                case 'd':
                    if (column == EncounterID.GetLength(0) - 1)
                    {
                        goto default;
                    }
                    column += 1;
                    if (impassable[row, column] == true)
                    {
                        column -= 1;
                    }
                    partyposition[row, column] = true;
                    break;
                case 's':
                    if (row == EncounterID.GetLength(1) - 1)
                    {
                        goto default;
                    }
                    row += 1;
                    if (impassable[row, column] == true)
                    {
                        row -= 1;
                    }
                    partyposition[row, column] = true;
                    break;
                default:
                    partyposition[row, column] = true;
                    break;
            }
            if (EncounterID[row,column] == 3)
            {
                revisitable[row, column] = true;
            }
            Encounter.explore(EncounterID[row, column], explored[row, column], ref hero, ref revisitable[row,column],Overworld);
            explored[row, column] = true;
        }
        void checksurroundings(int column, int row)
        {
            if (partyposition[column, row] == true)
            {
                int columnvaluemin = column - 1;
                int columnvaluemax = column + 1;
                int rowvaluemin = row - 1;
                int rowvaluemax = row + 1;
                if (column == 0)
                {
                    columnvaluemin = column;
                }
                if (column == EncounterID.GetLength(0) - 1)
                {
                    columnvaluemax = column;
                }
                if (row == 0)
                {
                    rowvaluemin = row;
                }
                if (row == EncounterID.GetLength(1) - 1)
                {
                    rowvaluemax = row;
                }
                for (int i = columnvaluemin; i <= columnvaluemax; i++)
                {
                    for (int j = rowvaluemin; j <= rowvaluemax; j++) 
                    {
                        surroundings[i, j] = true;
                    }
                }
            }
        }
    }
    class Encounter //eventy, import potworow
    {
        static public void explore(int EncounterID,bool explored, ref Hero hero, ref bool revisitable,SoundPlayer Overworld)
        {
            Random monster = new Random();
            string songpath;
            string MonsterImportData;
            string[] MonstersList;
            Console.ForegroundColor = ConsoleColor.White;
            if (explored == true && revisitable == false)
            {
                return;
            }
            Console.Clear();
            switch (EncounterID)
            {
                case 0:
                    //pole
                    break;
                case 1:
                    using(StreamReader sr = new StreamReader(Path.GetFullPath(@"MonstersRPG.ini")))
                    {
                        MonsterImportData = sr.ReadToEnd();
                        MonstersList = MonsterImportData.Split('\n');
                    }
                    songpath = @"Music\Battle.wav";
                    Monsters.fight(MonstersList[monster.Next(0,MonstersList.Length)],ref hero, Overworld,songpath);
                    break;
                case 2:
                    //trawa
                    break;
                case 3:
                    City.enter(ref hero);
                    break;
                case 4:
                    //woda
                    break;
                case 5:
                    //pole
                    break;
                case 6:
                    //pole
                    break;
                case 7:
                    //pole
                    break;
                case 8:
                    //pole
                    break;
                case 9:
                    //pole
                    break;
                case 1001:
                    songpath = @"Music\Boss.wav";
                    Monsters.fight("Mroczny rycerz;200;10;1001", ref hero, Overworld,songpath);
                    break;
            }
        }
    }
    class Monsters //walka
    {
        static public void fight(string mob, ref Hero hero, SoundPlayer Overworld, string songpath)
        {
            SoundPlayer Battle = new SoundPlayer(Path.GetFullPath(songpath));
            Battle.Play();
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Red;
            for(int height = 0; height < Console.WindowHeight; height++)
            {
                for (int width = 0; width < Console.WindowWidth; width++)
                {
                    Console.Write(" ");
                }
                Console.WriteLine();
                Thread.Sleep(25);
            }
            Console.ResetColor();
            Console.Clear();
            string[] mobstats = mob.Split(';');
            char move;
            Random HitChance = new Random();
            int MobHP = Convert.ToInt32(mobstats[1]);
            int MobFullHP = MobHP;
            int MobDMG = Convert.ToInt32(mobstats[2]);
            int MobDMGRoll;
            int MobID = Convert.ToInt32(mobstats[3]);
            string MobName = mobstats[0];
            int MoneyReward = MobHP / 3 + MobDMG / 3;
            int ExpReward = MobHP * 2 + MobDMG;
            Console.Clear();
            Console.WriteLine("zaatakowal cie" + " " + MobName);
            Console.ReadKey();
            while (MobHP > 0 && hero.HP > 0)
            {
                MobDMGRoll = MobDMG + HitChance.Next(-3, 4);
                if (HitChance.Next(1, 11) > 5 && MobDMGRoll>0)
                {                   
                    if (hero.Guard == true)
                    {
                        int blockedhit= MobDMGRoll - hero.Strength;
                        if (hero.Strength >= MobDMGRoll)
                        {
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("atak zablokowany!");
                            Console.ResetColor();
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.Clear();
                            hero.HP -= blockedhit;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("atak częściowo zablokowany! " + blockedhit + " dmg");
                            Console.ResetColor();
                            Console.ReadKey();
                        }
                        hero.Guard = false;
                    }
                    else
                    {
                        Console.Clear();
                        hero.HP -= MobDMGRoll;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(MobName + " uderza za " + MobDMGRoll + " dmg");
                        Console.ResetColor();
                        Console.ReadKey();
                    }
                }
                else
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(MobName + " Pudluje!");
                    Console.ResetColor();
                    Console.ReadKey();
                }
                hero.showstats();
                if (hero.HP <= 0)
                {
                    Overworld.PlayLooping();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("osuwasz sie na ziemie i padasz w kaluzy swojej krwi");
                    Console.ReadKey();
                    Console.ResetColor();
                    break;
                }
                Console.Write("HP przeciwnika:" + MobHP);
                if (MobID == 1001)
                {
                    Console.WriteLine(@"
  ,   A           {}
 / \, | ,        .--.
|    =|= >      /.--.\
 \ /` | `       |====|
  `   |         |`::`|  
      |     .-;`\..../`;_.-^-._
     /\\/  /  |...::..|`   :   `|
     |:'\ |   /'''::''|   .:.   |
      \ /\;-,/\   ::  |..:::::..|
      |\ <` >  >._::_.| ':::::' |
      | `--`  /   ^^  |   ':'   |
      |       |       \    :    /
      |       |        \   :   / 
      |       |___/\___|`-.:.-`
      |        \_ || _/    `
      |        <_ >< _>
      |        |  ||  |
      |        |  ||  |
      |       _\.:||:./_
      |      /____/\____\");
                }
                hero.showbars(MobHP, MobFullHP, ConsoleColor.Green, ConsoleColor.Red);
                Console.WriteLine("co robisz: a-atak b-blok u-ucieczka h-użycie mikstury ");
                try
                {
                    move = Convert.ToChar(Console.ReadLine());
                    
                
                switch (move)
                {
                    case 'b':
                        Console.WriteLine("przygotowujesz sie do bloku!");
                        hero.Guard = true;
                        break;
                    case 'a':
                        if (HitChance.Next(1, 11) > 2)
                        {
                            int hit = HitChance.Next(1, 5) * hero.Strength;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("trafiasz za " + hit + " dmg");
                            Console.ForegroundColor = ConsoleColor.White;
                            MobHP -= hit;
                        }
                        else
                        {
                            Console.WriteLine("Pudlujesz!");
                        }
                        break;
                        case 'u':
                            if (MobID == 1001)
                            {
                                Console.WriteLine("Nie możesz uciec przed mrokiem");
                                break;
                            }
                            if (HitChance.Next(1, 11) > 5)
                            {
                                Overworld.PlayLooping();
                                Console.Clear();
                                Console.WriteLine("udało ci sie zwiać!");
                                Console.ReadKey();
                                return;
                            }
                            else Console.WriteLine("nie udało ci sie uciec");
                            break;
                        case 'h':
                            if (hero.potions > 0)
                            {
                                Console.Clear();
                                hero.potions--;
                                hero.HP += 10;
                                Console.WriteLine("uleczyłeś 10HP");
                                Console.ReadKey();
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("nie masz mikstur!");
                                Console.ReadKey();
                            }
                            break;
                }
                    Console.ReadKey();
                }
                catch (FormatException) { }
                Console.Clear();
            }
            Console.WriteLine("koniec walki!");
            if (MobHP <= 0)
            {
                Overworld.PlayLooping();
                hero.Money += MoneyReward;
                hero.Experience += ExpReward;
                Console.WriteLine("otrzymujesz " + MoneyReward + " sztuki zlota oraz " + ExpReward + " pkt doświadczenia");
            }
            Console.ReadKey();
        }
    }
    public class City
    {
        public static void enter(ref Hero hero)
        {
            bool exit = false;
            while (exit == false)
            {
                int option;
                Console.Clear();
                Console.ResetColor();
                Console.WriteLine("wchodzisz do miasta, co robisz?\n1.Wyjdź\n2.Odpocznij w karczmie-3 sztuki złota\n3.Wejdź do sklepu\n4.Trenuj - 5 sztuk złota, 100 PD");
                try
                {
                    option = int.Parse(Console.ReadLine());
                }
                catch (FormatException)
                {
                    option = 1;
                }
                Console.Clear();
                switch (option)
                {
                    case 1:
                        exit = true;
                        break;
                    case 2:
                        if (hero.Money >= 3)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("czujesz sie wypoczęty! HP/MP zregenerowane");
                            hero.Money -= 3;
                            hero.HP = hero.HPMax;
                            hero.MP = hero.MPMax;
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("nie masz tyle pieniędzy!");
                            Console.ReadKey();
                        }
                        break;
                    case 3:
                            shop(ref hero);
                        break;
                    case 4:
                        if (hero.Weapon == true && hero.Strength < 5 && hero.Money>=5 && hero.Experience>=100)
                        {
                            Console.Clear();
                            hero.Strength++;
                            hero.HPMax += 10;
                            hero.Experience -= 100;
                            hero.Money -= 5;
                            Console.WriteLine("awansowałeś! siła i życie zwiększone");
                            Console.ReadKey();
                        }
                        else
                        {
                            if (hero.Weapon == false)
                            {
                                Console.Clear();
                                Console.WriteLine("do szkolenia potrzebujesz broni!");
                                Console.ReadKey();
                            }
                            if (hero.Strength == 5)
                            {
                                Console.Clear();
                                Console.WriteLine("umiesz już wszystko czego zwykli nauczyciele są w stanie cie nauczyć");
                                Console.ReadKey();
                            }
                            if(hero.Money<5 || hero.Experience < 100)
                            {
                                Console.Clear();
                                Console.WriteLine("nie masz wystarczająco zasobów żeby sie dalej uczyć!");
                                Console.ReadKey();
                            }
                        }
                        break;
                }
            }
        }
        public static void shop(ref Hero hero)
        {
            bool exit = false;
            while (exit == false)
            {
                Console.Clear();
                Console.WriteLine("co chcesz kupić?\n1.Miecz - 10 złota\n2.Zbroja - 10 złota\n3.Mikstura życia(10HP) - 5 złota\n4.Wyjdź ze sklepu");
                int option = int.Parse(Console.ReadLine());
                switch (option)
                {
                    case 1:
                        if (hero.Weapon == false && hero.Money>=10)
                        {
                            hero.Weapon = true;
                            hero.Strength++;
                            hero.Money -= 10;
                            Console.Clear();
                            Console.WriteLine("teraz posiadasz miecz!");
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("już posiadasz miecz lub nie masz pieniędzy");
                            Console.ReadKey();
                        }
                        break;
                    case 2:
                        if (hero.Armor == false && hero.Money >= 10) 
                        {
                            hero.Armor = true;
                            hero.HPMax += 20;
                            hero.Money -= 10;
                            Console.Clear();
                            Console.WriteLine("teraz posiadasz zbroje!");
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("już posiadasz zbroje lub nie masz pieniędzy");
                            Console.ReadKey();
                        }
                        break;
                    case 3:
                        if(hero.Money >= 5)
                        {
                            Console.Clear();
                            hero.Money -= 5;
                            hero.potions++;
                            Console.WriteLine("kupiłeś miksture życia");
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("masz za mało pieniędzy");
                            Console.ReadKey();
                        }
                        break;
                    case 4:
                        exit = true;
                        break;
                }
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            SoundPlayer Overworld = new SoundPlayer(Path.GetFullPath(@"Music\Overworld.wav"));
            Overworld.PlayLooping();
            Console.Title = "RPG";
            char playagain;
            bool play = true;
            while (play == true) {
                Console.SetWindowSize(100, 30);
                Hero hero = new Hero(30, 5, 1, 0, 0);
                Map world = new Map();
                world.Generate();
                world.refresh();
                while (hero.HP > 0)
                {
                    world.refresh();
                    hero.showstats();
                    Console.WriteLine("WSAD - nawigacja po mapie");
                    try
                    {
                        char side = char.Parse(Console.ReadLine());
                        world.movement(side, ref hero, ref world,Overworld);
                    }
                    catch (FormatException)
                    {
                    }
                    world.refresh();
                }
                Console.Clear();
                Console.WriteLine("przegrales! czy chcesz zagrac jeszcze raz? t/n");
                try
                {
                    playagain = char.Parse(Console.ReadLine());
              
                if (playagain == 't')
                {

                }
                else play = false;
                }
                catch (FormatException) { };
            }
        }
    }
}