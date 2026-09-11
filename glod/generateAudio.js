const fs = require('fs');
const { WaveFile } = require('wavefile');

const sampleRate = 44100;

function createSineBell(freqs, strikes, isFocus) {
  // Compute max length of chime needed based on last strike
  const maxStrikeDelay = strikes[strikes.length - 1];
  // Calculate max possible duration
  // Decay is up to 2.6s for fundamental + some padding
  const totalDuration = maxStrikeDelay + 2.6 + 0.1;
  const numSamples = Math.floor(sampleRate * totalDuration);
  const samples = new Float32Array(numSamples);

  const envelopeAttack = 0.012; // 12ms attack

  strikes.forEach((strikeDelay) => {
    freqs.forEach((freq, idx) => {
      // detune +0.09% per partial
      const detunedFreq = freq * (1 + 0.0009 * (idx + 1));

      // amplitude: gain / (idx + 1.6)
      const maxAmp = 0.45 / (idx + 1.6);

      // decay: 2.6s for fundamental, shortening 0.35s per partial above
      const decay = Math.max(0.6, 2.6 - (idx * 0.35));

      const startSample = Math.floor(strikeDelay * sampleRate);
      const attackEndSample = Math.floor((strikeDelay + envelopeAttack) * sampleRate);
      const decayEndSample = Math.floor((strikeDelay + decay) * sampleRate);

      for (let i = startSample; i < numSamples; i++) {
        const time = (i - startSample) / sampleRate;
        const totalTime = i / sampleRate;

        // Calculate envelope using exponential ramps like the Web Audio API
        // Simplified exponential approximation for rendering
        let env = 0.0001;
        if (i <= attackEndSample && i >= startSample) {
          // Attack phase (0.0001 to maxAmp)
          const t = (i - startSample) / (attackEndSample - startSample);
          env = 0.0001 * Math.pow(maxAmp / 0.0001, t);
        } else if (i > attackEndSample && i <= decayEndSample) {
          // Decay phase (maxAmp to 0.0001)
          const t = (i - attackEndSample) / (decayEndSample - attackEndSample);
          env = maxAmp * Math.pow(0.0001 / maxAmp, t);
        }

        if (env > 0) {
          const val = Math.sin(2 * Math.PI * detunedFreq * totalTime) * env;
          // Add this partial's value to the mix
          samples[i] += val;
        }
      }
    });
  });

  // Normalize and convert to 16-bit PCM
  let max = 0;
  for (let i = 0; i < samples.length; i++) {
    if (Math.abs(samples[i]) > max) max = Math.abs(samples[i]);
  }

  const pcmSamples = new Int16Array(numSamples);
  const masterCeiling = 0.5; // master ceiling is 0.5 as per spec

  for (let i = 0; i < samples.length; i++) {
    // Apply master ceiling logic
    let val = samples[i];
    // Scale appropriately (using 0.5 ceiling factor)
    val = val * (0.5 / (max === 0 ? 1 : max));
    // Convert to 16-bit Int (-32768 to 32767)
    pcmSamples[i] = Math.max(-32768, Math.min(32767, Math.floor(val * 32767)));
  }

  const wav = new WaveFile();
  wav.fromScratch(1, sampleRate, '16', pcmSamples);
  return wav.toBuffer();
}

// Generate Focus Bell (G4 stack: 392, 588, 784, 1176 Hz. Two strikes, 420 ms apart)
const focusFreqs = [392, 588, 784, 1176];
const focusStrikes = [0, 0.42];
const focusBuffer = createSineBell(focusFreqs, focusStrikes, true);
fs.writeFileSync('assets/focus-bell.wav', focusBuffer);
console.log('Created assets/focus-bell.wav');

// Generate Break Bell (C5 stack: 523, 784, 1047, 1568 Hz. One strike.)
const breakFreqs = [523, 784, 1047, 1568];
const breakStrikes = [0];
const breakBuffer = createSineBell(breakFreqs, breakStrikes, false);
fs.writeFileSync('assets/break-bell.wav', breakBuffer);
console.log('Created assets/break-bell.wav');
