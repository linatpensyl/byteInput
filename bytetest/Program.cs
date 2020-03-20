using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Ports;
namespace bytetest
{
    class Program
    {

        public static List<byte[]> recievedByte = new List<byte[]>();
        static async Task Main(string[] args)
        {

            int YOST_WAIT_TIME = 20;


            byte[] outputArray;


            SerialPort serialPort = new SerialPort("COM20", 115200, Parity.None, 8, StopBits.One);
            serialPort.WriteTimeout = 150;
            serialPort.ReadTimeout = 150;
            if (serialPort.IsOpen)
            {

            }
            else
            {
                serialPort.Open();
                //serialPort.WriteLine(":224\n");
                Thread.Sleep(YOST_WAIT_TIME);
                serialPort.BaseStream.ReadTimeout = 2000;
                bool _continue = true;

                serialPort.DiscardOutBuffer();
                serialPort.DiscardInBuffer();
                while (_continue)
                {


                    Console.WriteLine("Please input your byte array below:");
                    string input = Console.ReadLine().Trim().ToUpper();

                    if (string.Equals(input, "Q"))
                    {
                        _continue = false;
                        continue;
                    }
                    else if (string.Equals(input, "C"))
                    {
                        if (serialPort.BytesToRead > 0)
                        {
                            outputArray = new byte[serialPort.BytesToRead];
                            try
                            {
                                //serialPort.BaseStream.Read(streamoutputArray, 0, streamoutputArray.Length);

                                int byteReaded = await serialPort.BaseStream.ReadAsync(outputArray, 0, outputArray.Length);
                                Console.WriteLine("Byte Readed: " + byteReaded);
                                Console.WriteLine("Output: " + BitConverter.ToString(outputArray));

                            }
                            catch (TimeoutException ex)
                            {
                                Console.WriteLine("Time out!!");

                            }
                        }
                        else
                        {
                            Console.WriteLine("No byte to read");
                        }




                        continue;
                    }
                    else if (string.Equals(input, "I"))
                    {
                        serialPort.DiscardInBuffer();
                        continue;
                    }
                    else if (string.Equals(input, "O"))
                    {
                        serialPort.DiscardOutBuffer();

                        continue;
                    }


                    //Break down the input into byte array
                    string[] byteArrayString = input.Split();

                    byte[] inputByteArray = new byte[byteArrayString.Length + 1];

                    for (int i = 0; i < byteArrayString.Length; i++)
                    {
                        if (byteArrayString[i].Length != 2)
                        {
                            Console.WriteLine("Invalid length of argument");
                            goto EndOfLoop;
                        }
                        inputByteArray[i] = Convert.ToByte(byteArrayString[i].Substring(0, 2), 16);

                    }
                    addCheckSum(inputByteArray);
                    serialPort.BaseStream.WriteAsync(inputByteArray, 0, inputByteArray.Length);




                EndOfLoop:
                    Console.WriteLine();

                }





                return;

            }

            serialPort.Close();
            Thread.Sleep(2000);
        }

        static void printBufferArray(byte[] b)
        {
            Console.WriteLine(BitConverter.ToString(b));
        }

        static void addCheckSum(byte[] input)
        {
            byte checkSum = 0x00;
            for (int i = 1; i < input.Length - 1; i++)
            {
                checkSum += input[i];
            }
            input[input.Length - 1] = checkSum;
            Console.WriteLine("Byte Input: " + BitConverter.ToString(input));
        }
    }
}
