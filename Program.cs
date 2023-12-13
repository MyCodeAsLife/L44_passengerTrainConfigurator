﻿using System;
using System.Collections.Generic;

namespace L44_passengerTrainConfigurator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int CommandCreateDirection = 1;
            const int CommandSellTickets = 2;
            const int CommandFormTrain = 3;
            const int CommandSendTrain = 4;
            const int CommandShowAllVoyages = 5;
            const int CommandExit = 6;

            Random random = new Random();
            Office office = new Office();
            int lenghtDelimeter = 75;
            char delimeter = '=';
            bool isOpen = true;

            while (isOpen)
            {
                Console.Clear();
                Console.WriteLine($"{CommandCreateDirection} - Создать направление.\n{CommandSellTickets} - Продать билеты.\n{CommandFormTrain}" +
                                  $" - Сформировать поезд.\n{CommandSendTrain} - Отправить поезд.\n{CommandShowAllVoyages}" +
                                  $" - Показать все маршруты.\n{CommandExit} - Выйти из программы.");
                Console.WriteLine(new string(delimeter, lenghtDelimeter));

                Console.WriteLine($"Информция о формируемом рейсе:\nНаправление: {(office.CurrentDirection != null ? office.CurrentDirection.SourceCity : null)}" +
                                  $" - {(office.CurrentDirection != null ? office.CurrentDirection.DestinationCity : null)}\tБилетов продано: {office.SoldTickets}" +
                                  $"\tПоезд сформирован: {office.CurrentTrain != null}");
                Console.WriteLine(new string(delimeter, lenghtDelimeter));

                Console.Write("Выбирите действие: ");

                if (int.TryParse(Console.ReadLine(), out int numberMenu))
                {
                    Console.Clear();

                    switch (numberMenu)
                    {
                        case CommandCreateDirection:
                            office.CreateDirection();
                            break;

                        case CommandSellTickets:
                            office.SellTickets(random);
                            break;

                        case CommandFormTrain:
                            office.FormTrain();
                            break;

                        case CommandSendTrain:
                            office.SendTrain();
                            break;

                        case CommandShowAllVoyages:
                            office.ShowAllVoyage();
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
    }

    class Error
    {
        public static void Show()
        {
            Console.WriteLine("\nВы ввели некорректное значение.");
        }
    }

    class Office
    {
        private List<Voyage> _voyages = new List<Voyage>();
        private static int _count = 0;

        public Office()
        {
            MaxTickets = 300;
            SoldTickets = 0;
            CurrentTrain = null;
            CurrentDirection = null;
        }

        public Direction CurrentDirection { get; private set; }

        public Train CurrentTrain { get; private set; }

        public int MaxTickets { get; private set; }

        public int SoldTickets { get; private set; }

        public void DeleteCurrentVoyage()
        {
            SoldTickets = 0;
            CurrentDirection = null;
            CurrentTrain = null;
        }

        public void CreateDirection()
        {
            Console.Write("Введите город отправления: ");
            string sourceCityName = Console.ReadLine();

            Console.Write("Введите город назначения: ");
            string destinationCityName = Console.ReadLine();

            CurrentDirection = new Direction(sourceCityName, destinationCityName);
        }

        public void SellTickets(Random random)
        {
            SoldTickets = random.Next(MaxTickets + 1);
        }

        public void FormTrain()
        {
            CurrentTrain = new Train();

            if (SoldTickets < 1)
            {
                Console.WriteLine("Нет пассажиров для формирования поезда.");
                return;
            }

            if (SoldTickets >= (int)CarriageType.doubleEconomClass)
            {
                FillTrain(SoldTickets, CarriageType.doubleEconomClass, CurrentTrain);
            }
            else if (SoldTickets >= (int)CarriageType.compartment)
            {
                FillTrain(SoldTickets, CarriageType.compartment, CurrentTrain);
            }
            else if (SoldTickets >= (int)CarriageType.economClass)
            {
                FillTrain(SoldTickets, CarriageType.economClass, CurrentTrain);
            }
            else
            {
                FillTrain(SoldTickets, CarriageType.wagonLit, CurrentTrain);
            }
        }

        public void CreateVoyage()
        {
            if (CurrentDirection == null)
            {
                Console.WriteLine("Не создано напраления для рейса.");
                return;
            }

            if (CurrentTrain == null)
            {
                Console.WriteLine("Не сформирован поезд для рейса.");
                return;
            }

            _voyages.Add(new Voyage(CurrentDirection, CurrentTrain, _count++));
        }

        public void ShowAllVoyage()
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

        public void SendTrain()
        {
            CreateVoyage();
            DeleteCurrentVoyage();
        }

        private void FillTrain(int countPassengers, CarriageType carriage, Train train)
        {

            int countCarriage = countPassengers / (int)carriage;
            int restPeople = countPassengers % (int)carriage;

            for (int i = 0; i < countCarriage; i++)
                train.AddCarriage(new Carriage(carriage));

            if (restPeople > 0)
            {
                if (restPeople <= (int)CarriageType.wagonLit)
                    carriage = CarriageType.wagonLit;
                else if (restPeople <= (int)CarriageType.economClass)
                    carriage = CarriageType.economClass;
                else if (restPeople <= (int)CarriageType.compartment)
                    carriage = CarriageType.compartment;
                else
                    carriage = CarriageType.doubleEconomClass;

                train.AddCarriage(new Carriage(carriage));
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
        doubleEconomClass = 64,
        compartment = 54,
        economClass = 36,
        wagonLit = 18,
    }
}