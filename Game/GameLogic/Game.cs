﻿using System;
using System.Linq;
using Game.Views;

namespace Game.GameLogic
{
    public class Game
    {
        public Cell[] Map { get; set; }
        private GameStages stage = GameStages.Menu;
        public GameStages Stage => stage;
        public Player PlayerFirst { get; set; }
        public Player PlayerSecond { get; set; }
        public event Action<GameStages> StageChanged;
        private bool isFirstPlayerCurrent = false;
        public Player CurrentPlayer { get; set; }
        public event Action<Player> CurrentPlayerChanged;



        //Для игрока - человека нужно сделать выбор фигуры по нажатию ЛКМ
        //Для ИИ метод ChooseFigure должен быть написан в классе Player
        public Game()
        {
            Map = new Cell[31];
            MapFilling();
            PlayerFirst = new Player(ChipsType.Cone, Map);
            PlayerSecond = new Player(ChipsType.Coil, Map);
        }
        

        private void MapFilling()
        {
            for (var i = 1; i < 14; i += 2)
            {
                Map[i] = new Cell(new Figure(i, (i+1)/2, ChipsType.Cone));
                Map[i + 1] = new Cell(new Figure(i + 1, (i+1)/2, ChipsType.Coil));
            }

            for (var i = 16; i <= 25; i++)
            {
                Map[i] = new Cell(null);
            }
            
            Map[15] = new HouseOfRevival(null, 15);
            Map[26] = new HouseOfBeauty(null);
            Map[27] = new HouseOfWater(null);
            Map[28] = new HouseOfThreeTruths(null);
            Map[29] = new HouseOfIsidaAndNeftida(null);
            Map[30] = new HouseOfRaHorati(null);
        }

        public void ChangeStage(GameStages stage)
        {
            this.stage = stage;
            StageChanged?.Invoke(stage);
        }

        
        public void ChangeCurrentPlayer()
        {
            CurrentPlayer = isFirstPlayerCurrent ? PlayerSecond : PlayerFirst;
            //CurrentPlayerChanged?.Invoke(CurrentPlayer);
        }

        public void HumanMove(int figureNumber, Sticks sticks)
        {
            var stepCount = sticks.Throw();
            var currentFigure = CurrentPlayer.OwnFigures.Find(figure => figure.SerialNumber == figureNumber);
            if (!MakeStep(stepCount, Map, currentFigure)) return;
            if (sticks.ExtraMove) 
                HumanMove(currentFigure.SerialNumber, sticks);
            ChangeCurrentPlayer();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }
        
        public static bool MakeStep(int stepCount, Cell[] map, Figure figure)//Перенести часть? логики в Game
        {
            if (stepCount == 0) return false;
            var targetLocation = figure.Location + stepCount;
                
            if (map[figure.Location] is HouseOfWater)// Свойство дома
            {
                switch (stepCount)
                {
                    case 4:
                        map[figure.Location].State = null;
                        return true;
                    case 5:
                        return false;
                    default:
                        MoveToHouseOfRevival(map, figure); 
                        return true;
                }
            }
            
            if (targetLocation < map.Length 
                && map[targetLocation] is IFinalHouse 
                && !(map[figure.Location] is IFinalHouse))
            {
                StepToHouseOfBeauty(map, figure);
                return true;
            }

            if (map[figure.Location] is HouseOfBeauty)
            {
                if (stepCount == 5)
                    StepOut(stepCount, map, figure);
                else
                {
                    if (map[targetLocation].State == null)
                        SimpleStep(stepCount, map, figure);

                    else
                    {
                        var temp = map[targetLocation].State;
                        MoveToHouseOfRevival(map, temp);
                        SimpleStep(stepCount, map, figure);
                    }
                }

                return true;
            }//Вложенность
            

            if (map.Length - figure.Location <= 3)
                return StepOut(stepCount, map, figure);
            
            if (map[targetLocation].State == null)
            {
                SimpleStep(stepCount, map, figure);
                return true;
            }

            map[figure.Location + stepCount].State.IsFree = !CheckNeighbors(map, map[figure.Location + stepCount].State);
            
            if (map[targetLocation].State != null
                && map[targetLocation].State.IsFree && map[targetLocation].State.Type != figure.Type)
            {
                StepWithCut(stepCount, map, figure);
                return true;
            }
            
            return false;
        }

        private static void MoveToHouseOfRevival(Cell[] map, Figure figure)
        {
            for (var i = HouseOfRevival.Location; i >= 1; i--)
            {
                if (map[i].State != null) continue;
                map[i].State = figure;
                figure.Location = i;
                return;
            }
        }

        private static void SimpleStep(int stepCount, Cell[] map, Figure figure)
        {
            map[figure.Location + stepCount].State = map[figure.Location].State;
            map[figure.Location].State = null;
            figure.Location += stepCount;   
        }

        private static void StepWithCut(int stepCount, Cell[] map, Figure figure)
        {
            var temp = map[figure.Location + stepCount].State;
            map[figure.Location + stepCount].State = map[figure.Location].State;
            map[figure.Location].State = temp;
            figure.Location += stepCount;  
        }

        private static bool StepOut(int stepCount, Cell[] map, Figure figure)
        {
            if (stepCount != map.Length - figure.Location) return false;
            map[figure.Location].State = null;
            return true;
        }

        private static void StepToHouseOfBeauty(Cell[] map, Figure figure)
        {
            map[26].State = map[figure.Location].State;
            map[figure.Location].State = null;
            figure.Location = 26; 
        }

        private static bool CheckNeighbors(Cell[] map, Figure figure)
        {
            if (figure.Location + 1 < map.Length)
            {
                if (map[figure.Location + 1].State != null && map[figure.Location + 1].State.Type == figure.Type) return true;
                if (map[figure.Location - 1].State != null && map[figure.Location - 1].State.Type == figure.Type) return true;
            }

            return map[figure.Location - 1].State != null && map[figure.Location - 1].State.Type == figure.Type;
        }
    }
}