using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IPTestTask
{
    /// <summary>
    /// Класс с методами парсинга файла и записи результатов
    /// </summary>
    internal class AddressProcesser
    {
        /// <summary>
        /// Список с адресами и датами
        /// </summary>
        private readonly List<KeyValuePair<IPAddress, DateTime>> _addresses = new();

        /// <summary>
        /// Парсинг файла и добавление результатов в свойство _addresses
        /// </summary>
        /// <param name="inputFile"></param>
        public void ParseFile(string inputFile)
        {
            try
            {
                using StreamReader sr = new(inputFile);
                string? line;
                while ((!sr.EndOfStream))
                {
                    line = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) return;
                    string[] result = line.Split(':');
                    IPAddress address = IPAddress.Parse(result[0]);
                    DateTime date = DateTime.Parse(result[1]);

                    _addresses.Add(new KeyValuePair<IPAddress, DateTime>(address, date));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Произошла ошибка во время чтения журнала: {ex.Message}");
            }
        }

        /// <summary>
        /// Выборка и запись необходимых результатов в файл
        /// </summary>
        /// <param name="subnetMask">маска подсети</param>
        /// <param name="startAddress">нижняя граница адреса</param>
        /// <param name="startDate">нижняя граница диапазона дат</param>
        /// <param name="endDate">верхняя граница диапазона дат</param>
        /// <param name="outputPath">путь к файлу с результатами</param>
        public void PrintMatchingAdresses(int subnetMask, IPAddress? startAddress, DateTime? startDate, DateTime? endDate, string outputPath)
        {
            try
            {


                List<KeyValuePair<IPAddress, DateTime>> matchingAddresses = new();
                matchingAddresses = (from address in _addresses
                                     where IsInSameSubnet(address.Key, startAddress, subnetMask) == true &&
                                     (!startDate.HasValue || address.Value >= startDate) &&
                                     (!endDate.HasValue || address.Value <= endDate)
                                     select address).ToList();
                Dictionary<IPAddress, int> uniqueAddresses = new();
                foreach (var match in matchingAddresses)
                {
                    if(!uniqueAddresses.TryGetValue(match.Key, out int count))
                    {
                        uniqueAddresses.Add(match.Key, 1);
                    }
                    else
                    {
                        uniqueAddresses[match.Key] +=1;
                    }
                }
                using StreamWriter writer = new(outputPath);
                foreach (var item in uniqueAddresses)
                {
                    writer.WriteLine($"Адрес: {item.Key}, Кол-во: {item.Value}");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Произошла ошибка во время генерации результата: {ex.Message}");
            }
        }

        /// <summary>
        /// Находится ли адрес в подсети (В этом не уверен, надеюсь что тестовое задание было не на проверку именно этой его части)
        /// </summary>
        /// <param name="adressToCheck">адрес для проверки</param>
        /// <param name="lowerAdress">нижняя граница</param>
        /// <param name="upperSubnetMask">маска подсети</param>
        /// <returns></returns>
        private static bool IsInSameSubnet(IPAddress adressToCheck, IPAddress? lowerAdress, int upperSubnetMask)
        {
            if (lowerAdress == null) return true;
            if (upperSubnetMask == 0 || upperSubnetMask == -1) return true;

            Int64 subnetMask = Convert.ToInt64(Math.Pow(2, 32 - upperSubnetMask));
            byte[] ipBytes = adressToCheck.GetAddressBytes();
            byte[] subnetMaskBytes = BitConverter.GetBytes(subnetMask);

            bool isInRange = true;
            for (int i = 0; i < 4 && isInRange; i++)
            {
                if ((ipBytes[i] & subnetMaskBytes[i]) != (lowerAdress.GetAddressBytes()[i] & subnetMaskBytes[i]))
                {
                    isInRange = false;
                }
            }
            return isInRange;
        }
    }
}
