using System;

using UnityEngine;

using LunarPluginInternal;

namespace LunarPlugin.Test
{
    class TestAppImp : DefaultAppImp
    {
        private TestApp m_app;

        public TestAppImp(TestApp app)
        {
            m_app = app;
        }

        protected override void ResolveCommands()
        {
            if (Config.shouldResolveCommands)
            {
                base.ResolveCommands();
            }
        }

        protected override void ExecDefaultConfig()
        {
            if (Config.shouldExecDefaultConfig)
            {
                base.ExecDefaultConfig();
            }
        }

        protected override void RegisterCommandNotifications()
        {
            if (Config.shouldRegisterCommandNotifications)
            {
                base.RegisterCommandNotifications();
            }
        }

        public override void LogTerminal(string message)
        {
            if (Config.shouldLogTerminal)
            {
                Delegate.LogTerminal(message);
            }
        }

        public override void LogTerminal(string[] table)
        {
            if (Config.shouldLogTerminal)
            {
                Delegate.LogTerminal(table);
            }
        }

        public override void LogTerminal(Exception e, string message)
        {
            if (Config.shouldLogTerminal)
            {
                Delegate.LogTerminal(e, message);
            }
        }

        public override void ClearTerminal()
        {
            if (Config.shouldLogTerminal)
            {
                Delegate.ClearTerminal();
            }
        }

        protected override bool GetKeyDown(KeyCode key)
        {
            return false;
        }

        protected override bool GetKeyUp(KeyCode key)
        {
            return false;
        }

        protected override bool GetKey(KeyCode key)
        {
            return false;
        }

        TestAppConfig Config
        {
            get { return m_app.Config; }
        }

        ITestAppImpDelegate Delegate
        {
            get { return m_app.Delegate; }
        }
    }

    interface ITestAppImpDelegate
    {
        void LogTerminal(string message);
        void LogTerminal(string[] table);
        void LogTerminal(Exception e, string message);
        void ClearTerminal();
    }
}

