import { useState, useEffect, useRef } from 'react';
import { AppState } from 'react-native';
import * as Notifications from 'expo-notifications';
import { Audio } from 'expo-av';

const MODES = {
  focus: { totalSeconds: 25 * 60, label: 'Fokus' },
  short: { totalSeconds: 5 * 60, label: 'Kort paus' },
  long: { totalSeconds: 15 * 60, label: 'Lång paus' }
};

Notifications.setNotificationHandler({
  handleNotification: async () => ({
    shouldShowAlert: true,
    shouldPlaySound: true,
    shouldSetBadge: false,
  }),
});

export function useTimer() {
  const [mode, setMode] = useState('focus');
  const [isRunning, setIsRunning] = useState(false);
  const [remainingSeconds, setRemainingSeconds] = useState(MODES.focus.totalSeconds);
  const [cycle, setCycle] = useState(1);
  const [focusBell, setFocusBell] = useState(null);
  const [breakBell, setBreakBell] = useState(null);

  const expectedEndTimeRef = useRef(null);
  const appState = useRef(AppState.currentState);

  useEffect(() => {
    async function initAudio() {
      const { sound: fb } = await Audio.Sound.createAsync(require('./assets/focus-bell.wav'));
      const { sound: bb } = await Audio.Sound.createAsync(require('./assets/break-bell.wav'));
      setFocusBell(fb);
      setBreakBell(bb);

      // Request notification permissions
      await Notifications.requestPermissionsAsync();
    }
    initAudio();
    return () => {
      focusBell?.unloadAsync();
      breakBell?.unloadAsync();
    };
  }, []);

  useEffect(() => {
    let intervalId;
    if (isRunning) {
      intervalId = setInterval(() => {
        setRemainingSeconds((prev) => {
          if (prev <= 1) {
            handleComplete();
            return 0;
          }
          return prev - 1;
        });
      }, 1000);
    }
    return () => clearInterval(intervalId);
  }, [isRunning, mode, cycle]);

  useEffect(() => {
    const subscription = AppState.addEventListener('change', nextAppState => {
      if (
        appState.current.match(/inactive|background/) &&
        nextAppState === 'active'
      ) {
        // Returned to foreground
        if (isRunning && expectedEndTimeRef.current) {
          const now = Date.now();
          const msLeft = expectedEndTimeRef.current - now;
          if (msLeft <= 0) {
            setRemainingSeconds(0);
            handleComplete();
          } else {
            setRemainingSeconds(Math.ceil(msLeft / 1000));
          }
        }
      } else if (
        appState.current === 'active' &&
        nextAppState.match(/inactive|background/)
      ) {
        // Going to background
        if (isRunning) {
          expectedEndTimeRef.current = Date.now() + (remainingSeconds * 1000);
          scheduleNotification(remainingSeconds);
        }
      }
      appState.current = nextAppState;
    });

    return () => {
      subscription.remove();
    };
  }, [isRunning, remainingSeconds, mode]);

  const scheduleNotification = async (seconds) => {
    await Notifications.cancelAllScheduledNotificationsAsync();
    await Notifications.scheduleNotificationAsync({
      content: {
        title: mode === 'focus' ? 'Fokus klart' : 'Paus klar',
        body: 'Dags att byta pass.',
      },
      trigger: {
        type: Notifications.SchedulableTriggerInputTypes.TIME_INTERVAL,
        seconds: seconds,
      },
    });
  };

  const handleComplete = () => {
    setIsRunning(false);
    Notifications.cancelAllScheduledNotificationsAsync();
    playChime(mode === 'focus');
    skipPhase();
  };

  const playChime = async (isFocus = null) => {
    const isFocusSound = isFocus !== null ? isFocus : mode === 'focus';
    try {
      if (isFocusSound && focusBell) {
        await focusBell.replayAsync();
      } else if (!isFocusSound && breakBell) {
        await breakBell.replayAsync();
      }
    } catch (e) {
      console.warn("Audio playback failed", e);
    }
  };

  const toggleTimer = () => {
    if (!isRunning) {
      expectedEndTimeRef.current = Date.now() + (remainingSeconds * 1000);
      setIsRunning(true);
    } else {
      setIsRunning(false);
      expectedEndTimeRef.current = null;
      Notifications.cancelAllScheduledNotificationsAsync();
    }
  };

  const resetTimer = () => {
    setIsRunning(false);
    expectedEndTimeRef.current = null;
    Notifications.cancelAllScheduledNotificationsAsync();
    setRemainingSeconds(MODES[mode].totalSeconds);
  };

  const skipPhase = () => {
    let nextMode;
    if (mode === 'focus') {
      if (cycle >= 4) {
        nextMode = 'long';
        setCycle(1);
      } else {
        nextMode = 'short';
      }
    } else {
      nextMode = 'focus';
      if (mode === 'short' || mode === 'long') {
        if (mode === 'short') {
          setCycle(c => c + 1);
        }
      }
    }
    changeMode(nextMode);
  };

  const changeMode = (newMode) => {
    setMode(newMode);
    setIsRunning(false);
    expectedEndTimeRef.current = null;
    Notifications.cancelAllScheduledNotificationsAsync();
    setRemainingSeconds(MODES[newMode].totalSeconds);
  };

  return {
    mode,
    isRunning,
    remainingSeconds,
    totalSeconds: MODES[mode].totalSeconds,
    cycle,
    toggleTimer,
    resetTimer,
    skipPhase,
    changeMode,
    playChime,
    label: MODES[mode].label
  };
}
