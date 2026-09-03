import { describe, expect, it } from 'vitest';
import { sanitizeHtml } from './sanitizeHtml';

describe('sanitizeHtml', () => {
  it('removes executable markup and event handlers', () => {
    const html = '<img src=x onerror=alert(1)><script>alert(1)</script><p onclick="alert(1)">Safe</p>';

    const sanitized = sanitizeHtml(html);

    expect(sanitized).not.toContain('script');
    expect(sanitized).not.toContain('onerror');
    expect(sanitized).not.toContain('onclick');
    expect(sanitized).toContain('<p>Safe</p>');
  });
});
