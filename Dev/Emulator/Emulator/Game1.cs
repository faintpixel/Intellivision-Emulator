using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Emulator.Debugger;
using Intellivision.CPU;
using Intellivision.Memory;
using System.IO;

namespace Emulator
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        public static SpriteBatch SpriteBatch;
        public static ContentManager ContentManager;
        public static SpriteFont DebugFont;

        RegisterDisplay registers;
        CommandDisplay commandDisplay;

        MemoryMap memory;
        CP1610 cpu;
        bool _programRunning = true;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            ContentManager = Content;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
        }

        protected override void Initialize()
        {
            this.Window.Title = "Awesome Intellivision Emulator v0.0.0";
            this.IsMouseVisible = true;

            memory = new MemoryMap();
            cpu = new CP1610(ref memory);
            cpu.Halted_HALT += new CP1610.OutputSignalEvent(cpu_Halted_HALT);
            cpu.Log += new CP1610.LoggingEvent(cpu_Log);
            cpu.Registers[7] = 0x1000;

            registers = new RegisterDisplay(new Vector2(10, 10), ref cpu.Registers);
            commandDisplay = new CommandDisplay(new Vector2(10, 50));

            LoadSystemRom();
            RunEmulation();

            base.Initialize();
        }

        void cpu_Log(string message, Intellivision.CPU.LogType type)
        {
            commandDisplay.AddCommand(message);
        }

        private void LoadSystemRom()
        {
            BinaryReader reader = new BinaryReader(File.Open("system.bin", FileMode.Open));
            int pos = 0;
            UInt16 index = 0x1000;

            int length = (int)reader.BaseStream.Length;
            try
            {
                while (pos < length)
                {
                    byte[] word = reader.ReadBytes(2);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(word);

                    UInt16 data = BitConverter.ToUInt16(word, 0);
                    //UInt16 data = reader.ReadUInt16();
                    Console.Write(data.ToString("X") + ":");
                    memory.Write16BitsToAddress(index, data);
                    pos += sizeof(UInt16);
                    index += 1;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void RunEmulation()
        {
            while (_programRunning)
            {
                try
                {
                    //cpu.DEBUG_PRINT_JZINTV_STYLE_DEBUG_INFO();
                    cpu.ExecuteInstruction();
                    //cpu.DEBUG_PRINT_REGISTERS_AS_HEX();
                    //cpu.DEBUG_PRINT_FLAGS();
                    //Console.Write("> ");
                    //Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    cpu.Registers[7] += 1;
                    //Console.ReadLine();
                }
            }
        }


        void  cpu_Halted_HALT()
        {
            _programRunning = false;
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            DebugFont = ContentManager.Load<SpriteFont>("DebugFont");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            registers.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
                        
            SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            registers.Draw(gameTime);
            commandDisplay.Draw(gameTime);

            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
