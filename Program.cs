using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;

namespace L44_passengerTrainConfigurator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            Office office = new Office(random);

            office.Run();
        }
    }

    class Error
    {
        public static void Show()
        {
            Console.WriteLine("\nВы ввели некорректное значение.");
        }
    }

    class FormatOutput
    {
        static FormatOutput()
        {
            DelimiterSymbol = '=';
            DelimiterLenght = 75;
        }

        public static char DelimiterSymbol { get; private set; }

        public static int DelimiterLenght { get; private set; }
    }

    class Office
    {
        private const int CommandCreateDirection = 1;
        private const int CommandSellTickets = 2;
        private const int CommandFormTrain = 3;
        private const int CommandSendTrain = 4;
        private const int CommandShowAllVoyages = 5;
        private const int CommandExit = 6;

        private int _count = 0;
        private int _maxTickets;
        private int _soldTickets;

        private Random _random;
        private List<Voyage> _voyages = new List<Voyage>();
        private Direction _currentDirection;
        private Train _currentTrain;

        public Office(Random random)
        {
            _random = random;
            _maxTickets = 300;
            _soldTickets = 0;
            _currentTrain = null;
            _currentDirection = null;
        }

        public void Run()
        {
            bool isOpen = true;

            while (isOpen)
            {
                ShowMenu();
                ShowCurrentVoyageInfo();
                Console.Write("Выбирите действие: ");

                if (int.TryParse(Console.ReadLine(), out int numberMenu))
                {
                    Console.Clear();

                    switch (numberMenu)
                    {
                        case CommandCreateDirection:
                            CreateDirection();
                            break;

                        case CommandSellTickets:
                            SellTickets();
                            break;

                        case CommandFormTrain:
                            FormTrain();
                            break;

                        case CommandSendTrain:
                            SendTrain();
                            break;

                        case CommandShowAllVoyages:
                            ShowAllVoyage();
                            break;

                        case CommandExit:
                            isOpen = false;
                            continue;

                        default:
                            Error.Show();
                            break;
                    }
                }
                else
                {
                    Error.Show();
                }

                Console.ReadKey(true);
            }
        }

        private void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine($"{CommandCreateDirection} - Создать направление.\n{CommandSellTickets} - Продать билеты.\n{CommandFormTrain}" +
                              $" - Сформировать поезд.\n{CommandSendTrain} - Отправить поезд.\n{CommandShowAllVoyages}" +
                              $" - Показать все маршруты.\n{CommandExit} - Выйти из программы.");
            Console.WriteLine(new string(FormatOutput.DelimiterSymbol, FormatOutput.DelimiterLenght));
        }

        private void ShowCurrentVoyageInfo()
        {
            string sorceCity = (_currentDirection == null ? null : _currentDirection.SourceCity);
            string destinationCity = (_currentDirection != null ? _currentDirection.DestinationCity : null);
            bool isTrainFormed = (_currentTrain != null);

            Console.WriteLine($"Информция о формируемом рейсе:\nНаправление: {sorceCity}" +
                              $" - {destinationCity}\tБилетов продано: {_soldTickets}" +
                              $"\tПоезд сформирован: {isTrainFormed}");
            Console.WriteLine(new string(FormatOutput.DelimiterSymbol, FormatOutput.DelimiterLenght));
        }

        private void DeleteCurrentVoyage()
        {
            _soldTickets = 0;
            _currentDirection = null;
            _currentTrain = null;
        }

        private void CreateDirection()
        {
            Console.Write("Введите город отправления: ");
            string sourceCityName = Console.ReadLine();

            Console.Write("Введите город назначения: ");
            string destinationCityName = Console.ReadLine();

            _currentDirection = new Direction(sourceCityName, destinationCityName);
        }

        private void SellTickets()
        {
            if (_currentDirection == null)
            {
                Console.WriteLine("Направление еще не создано. Создайте направление.");
            }
            else
            {
                _soldTickets = _random.Next(_maxTickets + 1);
                Console.WriteLine($"Билетов удалось продать: {_soldTickets}.");
            }
        }

        private void CreateVoyage()
        {
            _voyages.Add(new Voyage(_currentDirection, _currentTrain, _count++));
        }

        private void ShowAllVoyage()
        {
            if (_voyages.Count > 0)
            {
                foreach (Voyage voyage in _voyages)
                {
                    Console.WriteLine($"Рейс №{voyage.Number}\tНаправление: {voyage.Direction.SourceCity} - {voyage.Direction.DestinationCity}" +
                                      $"\tКоличество вагонов: {voyage.Train.CountCarriages}");

                    for (int i = 0; i < voyage.Train.CountCarriages; i++)
                    {
                        Console.Write($"Номер вагона: {i + 1} - Тип: {voyage.Train.Carriages[i].Type} - Вместимость: {(int)voyage.Train.Carriages[i].Type}\t");

                        if (((i + 1) % 3) == 0)
                            Console.WriteLine();
                    }

                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Пока нет маршрутов.");
            }
        }

        private void SendTrain()
        {
            if (_currentTrain == null)
            {
                Console.WriteLine("Не сформирован поезд для рейса.");
            }
            else
            {
                CreateVoyage();
                DeleteCurrentVoyage();
            }
        }

        private void FormTrain()
        {
            if (_soldTickets > 0)
                FillTrain();
            else
                Console.WriteLine("Нет пассажиров для формирования поезда.");
        }

        private CarriageType SelectCarriage(int numberPassengers)
        {
            if (numberPassengers >= (int)CarriageType.DoubleEconomClass)
                return CarriageType.DoubleEconomClass;
            else if (numberPassengers >= (int)CarriageType.Compartment)
                return CarriageType.Compartment;
            else if (numberPassengers >= (int)CarriageType.EconomClass)
                return CarriageType.EconomClass;
            else
                return CarriageType.WagonLit;
        }

        private void FillTrain()
        {
            CarriageType carriage = SelectCarriage(this._soldTickets);
            int numberCarriages = _soldTickets / (int)carriage;
            int restPeople = _soldTickets % (int)carriage;
            _currentTrain = new Train();

            for (int i = 0; i < numberCarriages; i++)
                _currentTrain.AddCarriage(new Carriage(carriage));

            if (restPeople > 0)
            {
                carriage = SelectCarriage(restPeople);
                _currentTrain.AddCarriage(new Carriage(carriage));
            }
        }
    }

    class Voyage
    {
        public Voyage(Direction direction, Train train, int number)
        {
            Direction = direction;
            Train = train;
            Number = number;
        }

        public Train Train { get; private set; }

        public int Number { get; private set; }

        public Direction Direction { get; private set; }
    }

    class Direction
    {
        public Direction(string source, string destination)
        {
            SourceCity = source;
            DestinationCity = destination;
        }

        public string SourceCity { get; private set; }

        public string DestinationCity { get; private set; }
    }

    class Train
    {
        private List<Carriage> _carriages = new List<Carriage>();

        public int CountCarriages
        {
            get
            {
                return _carriages.Count;
            }
        }

        public List<Carriage> Carriages
        {
            get
            {
                List<Carriage> tempCarriages = new List<Carriage>();

                foreach (Carriage carriage in _carriages)
                    tempCarriages.Add(carriage);

                return tempCarriages;
            }
        }

        public void AddCarriage(Carriage carriage)
        {
            _carriages.Add(carriage);
        }
    }

    class Carriage
    {
        public Carriage(CarriageType type)
        {
            Type = type;
        }

        public CarriageType Type { get; private set; }
    }

    enum CarriageType
    {
        DoubleEconomClass = 64,
        Compartment = 54,
        EconomClass = 36,
        WagonLit = 18,
    }
}