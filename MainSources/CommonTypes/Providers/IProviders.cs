﻿using System;
using System.Collections.Generic;
using BaseLibrary;

namespace CommonTypes
{
    //Стандартный интерфейс для провайдера (Source, Archive или Receiver)
    public interface IProvider : IDisposable
    {
        //В конструкторе провайдера в него должны загружаться имя, свойства и логгер

        //Тип провайдера
        ProviderType Type { get; }
        //Код провайдера
        string Code { get; }
        //Имя экземпляра провайдера
        string Name { get; set; }
        //Cвойства провайдера
        string Inf { get; set; }
        //Кэш для идентификации соединения
        string Hash { get; }
        //Полное описание настроек провайдера для истории
        string Context { get; }
        //Каждому провайдеру передается логгер (поток)
        Logger Logger { get; set; }

        //Проверка соединения с провайдером, вызывается когда уже произошла ошибка для повторной проверки соединения
        //Возвращает true, если соединение установлено
        bool Check();

        //Настройка провайдера (через форму), возвращает строку с новыми настройками
        string Setup();
        //True, пока идет настройка
        bool IsSetup { get; set; }
        //Проверка соединения в форме настроек возвращает true, если соединение успешное
        bool CheckConnection();
        //Cтрока для вывода сообщения о последней проверке соединения
        string CheckConnectionMessage { get; }
        //Проверка корректности настроек, возвращает строку с ошибками, infDic - словарь настроек, nameDic - словарь имен свойств
        string CheckSettings(Dictionary<string, string> infDic, Dictionary<string, string> nameDic);
        //Словарь комманд открытия дилогов, ключи - имена свойств, вторые ключи - названия пунктов меню
        DicS<Dictionary<string, IMenuCommand>> MenuCommands { get; } 
        //Возвращает выпадающий список для поля настройки, props - словарь значение свойств, propname - имя свойства для ячейки со списком
        List<string> ComboBoxList(Dictionary<string, string> props, string propname);
    }

    //--------------------------------------------------------------------
    //Стандартный интерфейс для источников и имитаторов
    public interface IProviderSource : IProvider
    {
        //Список всех сигналов источника, сигналы можно только читать
        IDicSForRead<ProviderSignal> Signals { get; }
        //Чтение значений за период от beginRead до endRead
        void GetValues(DateTime beginRead, DateTime endRead);
    }

    //--------------------------------------------------------------------

    //Стандартный интерфейс для класса Source (источник исходных данных)
    public interface ISource : IProviderSource
    {
        //Подготовка провайдера к работе (во время PrepareCalc)
        void Prepare();
        //Добавить сигнал в список сигналов, принимает информацию по сигналу возвращает сам сигнал
        //Если сигнал уже есть, то его изменяет и возвращает и ничего не добавляет
        //signalInf, code, dataType - поля из Signals, idInClone - id в Signals клона, если синал используется для формирования клона
        ProviderSignal AddSignal(string signalInf, string code, DataType dataType, int idInClone = 0);
        //Удаляет из источника все сигналы
        void ClearSignals();

        //Получение интервала времени архива, если не удалось определить - возвращает null
        TimeInterval GetTime();
        //Список временных интервалов архива виде: BeginTime1, EndTime1, BeginTime2, EndTime2 и т. д.
        List<TimeInterval> TimeIntervals { get; }

        //Считать значения в клон, если задан cloneFile, cloneInf - настройки создания клона
        //Чтение значений за период от beginRead до endRead
        void MakeClone(DateTime beginRead, DateTime endRead, string cloneFile, string cloneInf);
    }

    //--------------------------------------------------------------------

    //Стандартный интерфейс для класса Receiver
    public interface IReceiver : IProvider
    {
        //Подготовка провайдера к работе (во время PrepareCalc)
        void Prepare();
        //Список сигналов приемника с мгновенными значениями
        IDicSForRead<ProviderSignal> Signals { get; }
        //Добавить сигнал приемника
        ProviderSignal AddSignal(string signalInf, string code, DataType dataType);
        //Отправить значения параметров на приемник
        void WriteValues();
        //Допускается передача списка мгновенных значений за один раз
        bool AllowListValues { get; }
    }
}