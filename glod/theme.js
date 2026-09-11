// The design system tokens
export const tokens = {
  ink: '#110E0C',
  ink2: '#191512',
  ink3: '#221C18',
  text: '#F2EAE1',
  muted: '#A0938A',
  line: 'rgba(242, 234, 225, 0.10)',
  lineStrong: 'rgba(242, 234, 225, 0.20)',
  ember: '#F26A3D',
  emberSoft: '#F8A07A',
  sage: '#7FBFA6',
  sageSoft: '#A8D7C6',
  s1: 4,
  s2: 6,
  s3: 9,
  s4: 13,
  s5: 19,
  s6: 28,
  s7: 40,
};

export const getAccent = (mode) => (mode === 'focus' ? tokens.ember : tokens.sage);
export const getAccentSoft = (mode) => (mode === 'focus' ? tokens.emberSoft : tokens.sageSoft);
