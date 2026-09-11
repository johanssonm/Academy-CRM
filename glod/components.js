import React from 'react';
import { View, Text, TouchableOpacity, StyleSheet } from 'react-native';
import Svg, { Line, RadialGradient, Defs, Rect, Circle, Stop } from 'react-native-svg';
import { tokens, getAccent, getAccentSoft } from './theme';

export function Dial({ litTicksCount, mode, progress }) {
  const accentColor = getAccent(mode);
  const cx = 100, cy = 100, rOuter = 92;
  const ticks = [];

  for (let i = 0; i < 60; i++) {
    const angle = (i * 6) * (Math.PI / 180) - (Math.PI / 2);
    const isMajor = i % 5 === 0;
    const length = isMajor ? 12 : 7;
    const strokeWidth = isMajor ? 2.5 : 1.25;

    const x1 = cx + rOuter * Math.cos(angle);
    const y1 = cy + rOuter * Math.sin(angle);
    const x2 = cx + (rOuter - length) * Math.cos(angle);
    const y2 = cy + (rOuter - length) * Math.sin(angle);

    const isLit = i < litTicksCount;
    const stroke = isLit ? accentColor : tokens.text;
    const opacity = isLit ? (isMajor ? 1 : 0.7) : 0.09;

    ticks.push(
      <Line
        key={i}
        x1={x1}
        y1={y1}
        x2={x2}
        y2={y2}
        stroke={stroke}
        strokeWidth={strokeWidth}
        strokeLinecap="round"
        opacity={opacity}
      />
    );
  }

  // Hearth glow opacity: 0.05 at start to 0.35 at end of session
  const glowOpacity = 0.05 + (progress) * 0.30;

  return (
    <View style={styles.dialContainer}>
      <Svg height="320" width="320" style={{ position: 'absolute', opacity: glowOpacity }}>
        <Defs>
          <RadialGradient id="grad" cx="50%" cy="50%" rx="50%" ry="50%">
            <Stop offset="0" stopColor={accentColor} stopOpacity="1" />
            <Stop offset="0.7" stopColor={accentColor} stopOpacity="0" />
          </RadialGradient>
        </Defs>
        <Circle cx="160" cy="160" r="160" fill="url(#grad)" />
      </Svg>
      <Svg height="310" width="310" viewBox="0 0 200 200">
        {ticks}
      </Svg>
    </View>
  );
}

export function ButtonPrimary({ onPress, label, mode, isActive }) {
  return (
    <TouchableOpacity
      activeOpacity={0.8}
      onPress={onPress}
      style={[
        styles.btnPrimary,
        { backgroundColor: isActive ? getAccentSoft(mode) : getAccent(mode) }
      ]}
    >
      <Text style={styles.btnPrimaryText}>{label}</Text>
    </TouchableOpacity>
  );
}

export function ButtonSecondary({ onPress, label }) {
  return (
    <TouchableOpacity
      activeOpacity={0.7}
      onPress={onPress}
      style={styles.btnSecondary}
    >
      <Text style={styles.btnSecondaryText}>{label}</Text>
    </TouchableOpacity>
  );
}

export function TabButton({ onPress, label, active, mode }) {
  return (
    <TouchableOpacity
      activeOpacity={1}
      onPress={onPress}
      style={[
        styles.tabBtn,
        active && { backgroundColor: getAccent(mode) }
      ]}
    >
      <Text style={[styles.tabBtnText, active && { color: tokens.ink }]}>{label}</Text>
    </TouchableOpacity>
  );
}

export function CyclePips({ cycle, mode }) {
  const pips = [];
  const accentColor = getAccent(mode);
  for (let i = 1; i <= 4; i++) {
    if (i < cycle) {
      // Completed pass
      pips.push(
        <View key={i} style={[styles.pipSolid, { backgroundColor: accentColor }]} />
      );
    } else if (i === cycle) {
      // Current pass (Ring)
      pips.push(
        <View key={i} style={[styles.pipRing, { borderColor: accentColor }]}>
          <View style={[styles.pipInner, { backgroundColor: accentColor }]} />
        </View>
      );
    } else {
      // Future pass
      pips.push(
        <View key={i} style={styles.pipEmpty} />
      );
    }
  }

  return (
    <View style={styles.cycleContainer}>
      <View style={styles.pipsRow}>{pips}</View>
      <Text style={styles.cycleText}>
        Pomodoro <Text style={styles.cycleTextHighlight}>{cycle}</Text> av <Text style={styles.cycleTextHighlight}>4</Text>
      </Text>
    </View>
  );
}

const styles = StyleSheet.create({
  dialContainer: {
    width: 310,
    height: 310,
    alignItems: 'center',
    justifyContent: 'center',
  },
  btnPrimary: {
    borderRadius: 999,
    minWidth: 140,
    height: 48,
    alignItems: 'center',
    justifyContent: 'center',
    paddingHorizontal: tokens.s6,
  },
  btnPrimaryText: {
    color: tokens.ink,
    fontFamily: 'FamiljenGrotesk-Bold',
    fontSize: 14.5,
    letterSpacing: 14.5 * 0.08,
    textTransform: 'uppercase',
  },
  btnSecondary: {
    backgroundColor: 'transparent',
    borderWidth: 1,
    borderColor: tokens.line,
    borderRadius: 999,
    paddingHorizontal: tokens.s4,
    height: 44,
    alignItems: 'center',
    justifyContent: 'center',
  },
  btnSecondaryText: {
    color: tokens.muted,
    fontFamily: 'FamiljenGrotesk-Medium',
    fontSize: 13,
  },
  tabBtn: {
    paddingVertical: tokens.s2,
    paddingHorizontal: 14,
    borderRadius: 999,
  },
  tabBtnText: {
    fontFamily: 'FamiljenGrotesk-SemiBold',
    fontSize: 12,
    letterSpacing: 12 * 0.1,
    textTransform: 'uppercase',
    color: tokens.muted,
  },
  cycleContainer: {
    alignItems: 'center',
    marginTop: tokens.s6,
  },
  pipsRow: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: tokens.s2,
    marginBottom: tokens.s2,
  },
  pipSolid: {
    width: 8,
    height: 8,
    borderRadius: 4,
  },
  pipRing: {
    width: 10,
    height: 10,
    borderRadius: 5,
    borderWidth: 1,
    alignItems: 'center',
    justifyContent: 'center',
  },
  pipInner: {
    width: 6,
    height: 6,
    borderRadius: 3,
  },
  pipEmpty: {
    width: 8,
    height: 8,
    borderRadius: 4,
    borderWidth: 1,
    borderColor: tokens.lineStrong,
  },
  cycleText: {
    fontFamily: 'FamiljenGrotesk-Regular',
    fontSize: 13,
    color: tokens.muted,
  },
  cycleTextHighlight: {
    color: tokens.text,
    fontFamily: 'FamiljenGrotesk-Medium',
  },
});
