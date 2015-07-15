﻿using System.Collections.Generic;

using UnityEngine;

using LunarPlugin;

namespace LunarPluginInternal
{
    class DefaultAppImp : AppImp, IUpdatable, IDestroyable, ICCommandDelegate
    {
        private readonly CommandProcessor m_processor;
        private readonly TimerManager m_timerManager;
        private readonly NotificationCenter m_notificationCenter;
        private readonly UpdatableList m_updatables;

        public DefaultAppImp()
        {
            m_timerManager = CreateTimerManager();
            m_notificationCenter = CreateNotificationCenter();
            m_processor = CreateCommandProcessor();

            m_updatables = new UpdatableList(2);
            m_updatables.Add(m_timerManager);
            m_updatables.Add(UpdateBindings);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

        public virtual void Start()
        {
            ResolveCommands();

            TimerManager.ScheduleTimerOnce(delegate()
            {
                ExecDefaultConfig();
                RegisterCommandNotifications();
            });
        }

        public void Stop()
        {
            // TODO: cancel all and release resources
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Inheritance

        protected virtual void ResolveCommands()
        {
            CRegistery.ResolveCommands();
        }

        protected virtual void ExecDefaultConfig()
        {
            App.ExecCommand("exec default.cfg");
        }

        protected virtual void RegisterCommandNotifications()
        {
            // cvar value changed
            m_notificationCenter.Register(CCommandNotifications.CVarValueChanged, delegate(Notification n)
            {
                bool manual = n.Get<bool>(CCommandNotifications.KeyManualMode);

                CVar cvar = n.Get<CVar>(CCommandNotifications.CVarValueChangedKeyVar);
                Assert.IsNotNull(cvar);

                OnCVarValueChanged(cvar, manual);
            });

            // binding changed
            m_notificationCenter.Register(CCommandNotifications.CBindingsChanged, delegate(Notification n)
            {
                bool manual = n.Get<bool>(CCommandNotifications.KeyManualMode);
                OnCBindingsChanged(manual);
            });

            // alias changed
            m_notificationCenter.Register(CCommandNotifications.CAliasesChanged, delegate(Notification n)
            {
                bool manual = n.Get<bool>(CCommandNotifications.KeyManualMode);
                OnCAliasesChanged(manual);
            });
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region IUpdatable

        public void Update(float dt)
        {
            m_updatables.Update(dt);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region IDestroyable

        public void Destroy()
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Commands

        public bool ExecCommand(string commandLine, bool manual)
        {
            return m_processor.TryExecute(commandLine, manual);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region ICCommandDelegate implementation

        public virtual void LogTerminal(string message)
        {
        }

        public virtual void LogTerminal(string[] table)
        {
        }

        public virtual void LogTerminal(System.Exception e, string message)
        {
        }

        public virtual void ClearTerminal()
        {
        }

        public virtual bool ExecuteCommandLine(string commandLine, bool manual = false)
        {
            return ExecCommand(commandLine, manual);
        }

        public virtual void PostNotification(CCommand cmd, string name, params object[] data)
        {
            m_notificationCenter.Post(cmd, name, data);
        }

        public virtual bool IsPromptEnabled
        {
            get { return false; }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Factory methods

        protected virtual TimerManager CreateTimerManager()
        {
            return TimerManager.SharedInstance;
        }

        protected virtual NotificationCenter CreateNotificationCenter()
        {
            return NotificationCenter.SharedInstance;
        }

        protected virtual CommandProcessor CreateCommandProcessor()
        {
            CommandProcessor processor = new CommandProcessor();
            processor.CommandDelegate = this;
            return processor;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Config

        protected virtual void OnCVarValueChanged(CVar cvar, bool manual)
        {
            if (manual)
            {
                ScheduleSaveConfig();
            }
        }

        protected virtual void OnCBindingsChanged(bool manual)
        {
            if (manual)
            {
                ScheduleSaveConfig();
            }
        }

        protected virtual void OnCAliasesChanged(bool manual)
        {
            if (manual)
            {
                ScheduleSaveConfig();
            }
        }

        protected virtual void ScheduleSaveConfig()
        {
            m_timerManager.ScheduleOnce(SaveConfig);
        }

        protected virtual void SaveConfig()
        {
            App.ExecCommand("writeconfig default.cfg");
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Bindings

        private void UpdateBindings(float delta)
        {
            IList<CBinding> bindings = CBindings.BindingsList;
            for (int i = 0; i < bindings.Count; ++i)
            {
                KeyCode key = bindings[i].key;
                if (Input.GetKeyDown(key))
                {
                    if (IsValidModifiers(bindings[i].shortCut))
                    {
                        string commandLine = bindings[i].cmdKeyDown;
                        ExecCommand(commandLine, false);
                    }
                }
                else if (Input.GetKeyUp(key))
                {
                    if (IsValidModifiers(bindings[i].shortCut))
                    {
                        string commandLine = bindings[i].cmdKeyUp;
                        if (commandLine != null)
                        {
                            ExecCommand(commandLine, false);
                        }
                    }
                }
            }
        }

        private bool IsValidModifiers(CShortCut shortCut)
        {
            if (shortCut.IsShift ^ (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) return false;
            if (shortCut.IsControl ^ (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))) return false;
            if (shortCut.IsAlt ^ (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))) return false;
            if (shortCut.IsCommand ^ (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand))) return false;

            return true;
        }

        #endregion
    }
}

