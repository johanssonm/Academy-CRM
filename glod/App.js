import React, { useState, useEffect } from 'react';
import { StatusBar } from 'expo-status-bar';
import { StyleSheet, Text, View, SafeAreaView } from 'react-native';
import * as Font from 'expo-font';
import { tokens, getAccent } from './theme';
import { useTimer } from './useTimer';
import { Dial, ButtonPrimary, ButtonSecondary, TabButton, CyclePips } from './components';

export default function App() {
  const [fontsLoaded, setFontsLoaded] = useState(false);

  const timer = useTimer();

  useEffect(() => {
    async function loadFonts() {
      try {
        await Font.loadAsync({
          'Fraunces-Light': require('./assets/Fraunces-VariableFont_SOFT,WONK,opsz,wght.ttf'),
          'FamiljenGrotesk-Regular': require('./assets/FamiljenGrotesk-VariableFont_wght.ttf'),
          'FamiljenGrotesk-Medium': require('./assets/FamiljenGrotesk-VariableFont_wght.ttf'),
          'FamiljenGrotesk-SemiBold': require('./assets/FamiljenGrotesk-VariableFont_wght.ttf'),
          'FamiljenGrotesk-Bold': require('./assets/FamiljenGrotesk-VariableFont_wght.ttf'),
        });
        setFontsLoaded(true);
      } catch(e) {
        console.error("Font loading error:", e);
      }
    }
    loadFonts();
  }, []);

  if (!fontsLoaded) return null;

  const mins = Math.floor(timer.remainingSeconds / 60);
  const secs = timer.remainingSeconds % 60;

  const m1 = Math.floor(mins / 10);
  const m2 = mins % 10;
  const s1 = Math.floor(secs / 10);
  const s2 = secs % 10;

  const progressElapsed = 1 - (timer.remainingSeconds / timer.totalSeconds);
  const litTicksCount = Math.round((timer.remainingSeconds / timer.totalSeconds) * 60);

  return (
    <SafeAreaView style={styles.container}>
      <StatusBar style="light" backgroundColor={tokens.ink} />

      {/* Header */}
      <View style={styles.header}>
        <View style={styles.headerLeft}>
          <View style={[styles.statusDot, { backgroundColor: getAccent(timer.mode) }]} />
          <Text style={styles.headerTitle}>Glöd</Text>
        </View>
      </View>

      <View style={styles.main}>
        {/* Mode Segments */}
        <View style={styles.segmentedControl}>
          <TabButton
            active={timer.mode === 'focus'}
            label="Fokus"
            mode={timer.mode}
            onPress={() => timer.changeMode('focus')}
          />
          <TabButton
            active={timer.mode === 'short'}
            label="Kort paus"
            mode={timer.mode}
            onPress={() => timer.changeMode('short')}
          />
          <TabButton
            active={timer.mode === 'long'}
            label="Lång paus"
            mode={timer.mode}
            onPress={() => timer.changeMode('long')}
          />
        </View>

        {/* Dial & Typographic Readout */}
        <View style={styles.dialWrapper}>
          <Dial litTicksCount={litTicksCount} mode={timer.mode} progress={progressElapsed} />

          <View style={styles.readout}>
            <Text style={styles.phaseLabel}>{timer.label}</Text>

            <View style={styles.timeDisplay}>
              <Text style={styles.digit}>{m1}</Text>
              <Text style={styles.digit}>{m2}</Text>
              <Text style={[styles.colon, { color: getAccent(timer.mode) }]}>:</Text>
              <Text style={styles.digit}>{s1}</Text>
              <Text style={styles.digit}>{s2}</Text>
            </View>

            <Text style={styles.stickStatus}>
              <Text style={styles.stickStatusLit}>{litTicksCount}</Text> av 60 stickor brinner
            </Text>
          </View>
        </View>

        {/* Cycle Pips */}
        <CyclePips cycle={timer.cycle} mode={timer.mode} />
      </View>

      {/* Footer Controls */}
      <View style={styles.footer}>
        <View style={styles.controlsRow}>
          <ButtonSecondary label="Återställ" onPress={timer.resetTimer} />
          <ButtonPrimary
            label={timer.isRunning ? 'Pausa' : 'Starta'}
            mode={timer.mode}
            isActive={timer.isRunning}
            onPress={timer.toggleTimer}
          />
          <ButtonSecondary label="Hoppa över" onPress={() => { timer.playChime(); timer.skipPhase(); }} />
        </View>
      </View>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: tokens.ink,
    justifyContent: 'space-between',
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingHorizontal: tokens.s5,
    paddingVertical: tokens.s3,
    borderBottomWidth: 1,
    borderBottomColor: tokens.line,
  },
  headerLeft: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: tokens.s3,
  },
  statusDot: {
    width: 10,
    height: 10,
    borderRadius: 5,
  },
  headerTitle: {
    fontFamily: 'FamiljenGrotesk-Bold',
    fontSize: 12,
    letterSpacing: 12 * 0.18,
    textTransform: 'uppercase',
    color: tokens.text,
  },
  main: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
  },
  segmentedControl: {
    flexDirection: 'row',
    backgroundColor: tokens.ink2,
    borderWidth: 1,
    borderColor: tokens.line,
    borderRadius: 999,
    padding: tokens.s1,
    marginBottom: tokens.s6,
  },
  dialWrapper: {
    alignItems: 'center',
    justifyContent: 'center',
    width: 320,
    height: 320,
  },
  readout: {
    position: 'absolute',
    alignItems: 'center',
  },
  phaseLabel: {
    fontFamily: 'FamiljenGrotesk-SemiBold',
    fontSize: 11.5,
    letterSpacing: 11.5 * 0.20,
    textTransform: 'uppercase',
    color: tokens.muted,
    marginBottom: tokens.s1,
  },
  timeDisplay: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
  },
  digit: {
    fontFamily: 'Fraunces-Light',
    fontSize: 62,
    color: tokens.text,
    width: 62 * 0.60,
    textAlign: 'center',
    letterSpacing: -0.62,
  },
  colon: {
    fontFamily: 'Fraunces-Light',
    fontSize: 62,
    width: 62 * 0.30,
    textAlign: 'center',
    paddingBottom: 4,
  },
  stickStatus: {
    fontFamily: 'FamiljenGrotesk-Regular',
    fontSize: 12,
    color: tokens.muted,
    marginTop: tokens.s2,
  },
  stickStatusLit: {
    fontFamily: 'FamiljenGrotesk-Medium',
    color: tokens.text,
  },
  footer: {
    paddingBottom: tokens.s6,
    paddingHorizontal: tokens.s4,
    alignItems: 'center',
  },
  controlsRow: {
    flexDirection: 'row',
    width: '100%',
    justifyContent: 'space-between',
    alignItems: 'center',
    gap: tokens.s3,
  },
});
