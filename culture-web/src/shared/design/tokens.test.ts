import { describe, expect, it } from 'vitest';
import { colors } from './tokens';

describe('design tokens', () => {
  it('exposes the primary mockup palette', () => {
    expect(colors.primary).toBe('#5376BA');
    expect(colors.yellow).toBe('#D9CB0C');
    expect(colors.ink).toBe('#1F2120');
  });
});
