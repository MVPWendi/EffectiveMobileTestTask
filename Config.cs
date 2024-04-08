using System.Net;

namespace IPTestTask
{
    /// <summary>
    /// Класс с настройками приложения (Парсит аргументы, проверяет корректность данных и т.д.
    /// </summary>
    internal class Config
    {
        /// <summary>
        /// Путь к исходному журналу
        /// </summary>
        private string _logPath;
        /// <summary>
        /// Путь к результату
        /// </summary>
        private string _outputPath;
        /// <summary>
        /// Нижняя граница адресов
        /// </summary>
        private IPAddress? _startAddress;
        /// <summary>
        /// Верхняя граница (маска подсети в 10 виде)
        /// </summary>
        private int _endAddress;
        /// <summary>
        /// Нижняя граница временного интервала
        /// </summary>
        private DateTime? _timeStart;
        /// <summary>
        /// Верхняя граница временного интервала
        /// </summary>
        private DateTime? _timeEnd;

        /// <summary>
        /// Проведена ли "установка" (Все необходимые параметры установлены)
        /// </summary>
        private bool _setupCompleted;
        public string LogPath
        {
            get
            {
                if (_setupCompleted == false) throw new InvalidOperationException("Не произведена настройка программы");
                return _logPath;
            }
        }
        public string OutputPath
        {
            get
            {
                if (_setupCompleted == false) throw new InvalidOperationException("Не произведена настройка программы");
                return _outputPath;
            }
        }
        public IPAddress? StartAddress
        {
            get
            {
                if (_setupCompleted == false) throw new InvalidOperationException("Не произведена настройка программы");
                return _startAddress;
            }
        }
        public int EndAddress
        {
            get
            {
                if (_setupCompleted == false) throw new InvalidOperationException("Не произведена настройка программы");
                return _endAddress;
            }
        }
        public DateTime? TimeStart
        {
            get
            {
                if (_setupCompleted == false) throw new InvalidOperationException("Не произведена настройка программы");
                return _timeStart;
            }
        }
        public DateTime? TimeEnd
        {
            get
            {
                if (_setupCompleted == false) throw new InvalidOperationException("Не произведена настройка программы");
                return _timeEnd;
            }
        }

        public Config()
        {
            _setupCompleted = false;
            _logPath = "";
            _outputPath = "";
            _startAddress = null;
            _endAddress = -1;

        }

        /// <summary>
        /// Провести установку (парсинг аргументов)
        /// </summary>
        /// <param name="args">аргументы командной строки</param>
        /// <param name="error">сообщение об ошибке, если что-то пошло не так</param>
        /// <returns>результат парсинга</returns>
        /// <exception cref="ArgumentException"></exception>
        public bool SetupConfig(string[] args, out string error)
        {
            error = "";
            try
            {


                bool parseRes = false;
                for (int i = 0; i < args.Length; i += 2)
                {
                    switch (args[i])
                    {
                        case "--file-log":
                            _logPath = args[i + 1];
                            if (!File.Exists(_logPath)) throw new ArgumentException("Такого файла с журналом адресов не существует");
                            break;
                        case "--file-output":
                            _outputPath = args[i + 1];
                            break;
                        case "--address-start":
                            parseRes = IPAddress.TryParse(args[i + 1], out _startAddress);
                            if (!parseRes) throw new ArgumentException("Неверно указана нижняя граница диапазона адресов");
                            break;
                        case "--address-mask":
                            parseRes = int.TryParse(args[i + 1], out _endAddress);
                            if (!parseRes || _endAddress >32 || _endAddress<0) throw new ArgumentException("Неверно указана нижняя граница диапазона адресов");
                            break;
                        case "--time-start":
                            parseRes = DateTime.TryParse(args[i + 1], out DateTime timeStart);
                            if (!parseRes) throw new ArgumentException("Неверно указана нижняя граница временного интервала");
                            _timeStart = timeStart;
                            break;
                        case "--time-end":
                            parseRes = DateTime.TryParse(args[i + 1], out DateTime timeEnd);
                            if (!parseRes) throw new ArgumentException("Неверно указана нижняя граница временного интервала");
                            _timeEnd = timeEnd;
                            break;
                        default:
                            break;
                    }
                }
                CheckForRequiredParameters();
                _setupCompleted = true;
                return true;
            }
            catch(Exception ex)
            {
                error = $"Произошла ошибка при попытке парсинга аргументов: {ex.Message}";
                return false;
            }
        }
        /// <summary>
        /// Проверить есть ли все необходимые значения параметров, вызывает исключение, если нет
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        private void CheckForRequiredParameters()
        {
            if (_timeEnd == null || _timeStart == null) throw new ArgumentException("Отстутствуют границы временного интервала");
            if (_timeEnd < _timeStart) throw new ArgumentException("Верхняя граница временного интервала ниже верхней");
            if (string.IsNullOrEmpty(_logPath)) throw new ArgumentException("Отсутствует параметр с путём к файлу журнала адресов");
            if (string.IsNullOrEmpty(_outputPath)) throw new ArgumentException("Отсутствует параметр с путём к файлу результата");
            if(_startAddress == null && _endAddress == -1) throw new ArgumentException("Нельзя использовать верхнюю границу, если не указана нижняя");
        }
    }


    
}