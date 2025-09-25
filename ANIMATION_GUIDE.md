# Animation Implementation Documentation

## Overview

This document explains the animation system implemented in the LEN E-Whistleblowing System using AOS (Animate On Scroll) library and custom CSS animations.

## Animation Types

### 1. Initial Load Animation (Slide In)

- **Target**: Hero section (`#home`)
- **Effect**: Slides in from left to center
- **Duration**: 1.2 seconds
- **Implementation**: CSS keyframe animation `slideInFromLeft`

### 2. Hero Content Staggered Animation

- **Target**: Hero content elements (title, description, buttons)
- **Effect**: Fade in with upward movement
- **Timing**: Staggered delays (0.3s, 0.6s, 0.9s, 1.2s, 1.4s)
- **Implementation**: CSS keyframe animation `fadeInUp`

### 3. Scroll Reveal Animations

- **Library**: AOS (Animate On Scroll) v2.3.1
- **Trigger**: When elements enter viewport
- **Effects Used**:
  - `fade-up`: Fade in with upward movement
  - `zoom-in`: Scale up from center
  - `fade-right`: Fade in with rightward movement

## Implementation Details

### CSS Animations

```css
@keyframes slideInFromLeft {
  0% {
    transform: translateX(-100%);
    opacity: 0;
  }
  100% {
    transform: translateX(0);
    opacity: 1;
  }
}

@keyframes fadeInUp {
  0% {
    transform: translateY(50px);
    opacity: 0;
  }
  100% {
    transform: translateY(0);
    opacity: 1;
  }
}
```

### AOS Configuration

```javascript
AOS.init({
  duration: 800,
  easing: "ease-in-out",
  once: true,
  offset: 120,
});
```

### Blazor Integration

- AOS is initialized via JavaScript interop
- `OnAfterRenderAsync` ensures proper initialization after component render
- `initializeAOS()` function refreshes AOS when needed

## Animation Timing

1. **Page Load** (0s): Hero section slides in from left
2. **0.3s**: Hero title fades in from bottom
3. **0.6s**: Hero description fades in from bottom
4. **0.9s**: Hero actions container fades in
5. **1.2s**: "Buat Laporan" button fades in
6. **1.4s**: "Cek Status" button fades in
7. **On Scroll**: About section elements animate based on viewport position

## Performance Considerations

- Animations use `transform` and `opacity` for GPU acceleration
- `once: true` prevents animations from repeating
- Minimal JavaScript footprint with CDN delivery
- CSS animations are hardware accelerated

## Browser Support

- Modern browsers (Chrome, Firefox, Safari, Edge)
- Graceful degradation for older browsers
- CSS animations work even if JavaScript is disabled

## Customization

### Animation Speed

Modify `duration` values in AOS config or CSS animation durations.

### Animation Delays

Adjust `data-aos-delay` attributes or CSS `animation-delay` properties.

### Animation Types

Change `data-aos` attributes to use different AOS animations:

- `fade-up`, `fade-down`, `fade-left`, `fade-right`
- `zoom-in`, `zoom-out`
- `slide-up`, `slide-down`, `slide-left`, `slide-right`
- `flip-left`, `flip-right`, `flip-up`, `flip-down`

## Troubleshooting

### Animations Not Working

1. Check browser console for JavaScript errors
2. Verify AOS CSS and JS are loaded
3. Ensure `initializeAOS()` is called after component render
4. Check that elements have proper `data-aos` attributes

### Performance Issues

1. Reduce animation durations
2. Use `will-change: transform` for better GPU acceleration
3. Minimize the number of simultaneously animating elements
