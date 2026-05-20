(function () {
    'use strict';

    function getDecimalPlaces(n) {
        var s = n.toString();
        var i = s.indexOf('.');
        return i === -1 ? 0 : s.length - i - 1;
    }

    function initSpinners() {
        // querySelectorAll returns a static snapshot — dynamically-created
        // spinner-step inputs will never appear here, but the guard is kept
        // for safety if initSpinners() is ever called more than once.
        document.querySelectorAll('input[type="number"]').forEach(function (input) {
            if (input.classList.contains('spinner-step')) return;
            if (input.closest('.num-spinner')) return;

            var stepAttr = parseFloat(input.getAttribute('step'));
            var defaultStep = isNaN(stepAttr) ? 1 : stepAttr;
            var min = input.hasAttribute('min') ? parseFloat(input.getAttribute('min')) : null;
            var max = input.hasAttribute('max') ? parseFloat(input.getAttribute('max')) : null;

            // ── Wrapper ──────────────────────────────────────────────────
            var wrapper = document.createElement('div');
            wrapper.className = 'num-spinner';
            input.parentNode.insertBefore(wrapper, input);
            wrapper.appendChild(input);

            // ── Minus button (before the input) ──────────────────────────
            var btnMinus = document.createElement('button');
            btnMinus.type = 'button';
            btnMinus.className = 'spinner-btn spinner-minus';
            btnMinus.setAttribute('aria-label', 'Decrease');
            btnMinus.textContent = '\u2212'; // minus sign (−)
            wrapper.insertBefore(btnMinus, input);

            // ── Plus button (after the input) ────────────────────────────
            var btnPlus = document.createElement('button');
            btnPlus.type = 'button';
            btnPlus.className = 'spinner-btn spinner-plus';
            btnPlus.setAttribute('aria-label', 'Increase');
            btnPlus.textContent = '+';
            wrapper.appendChild(btnPlus);

            // ── Step control ─────────────────────────────────────────────
            var stepWrap = document.createElement('div');
            stepWrap.className = 'spinner-step-wrap';

            var stepLabel = document.createElement('span');
            stepLabel.className = 'spinner-step-label';
            stepLabel.textContent = 'Step';

            var stepInput = document.createElement('input');
            stepInput.type = 'number';
            stepInput.className = 'spinner-step input-barbershop';
            stepInput.value = defaultStep;
            stepInput.min = '0.001';
            stepInput.step = 'any';
            stepInput.setAttribute('aria-label', 'Step size');

            stepWrap.appendChild(stepLabel);
            stepWrap.appendChild(stepInput);
            wrapper.appendChild(stepWrap);

            // ── Logic ─────────────────────────────────────────────────────
            function clamp(val) {
                if (min !== null && val < min) val = min;
                if (max !== null && val > max) val = max;
                return val;
            }

            function getStep() {
                return Math.abs(parseFloat(stepInput.value) || defaultStep);
            }

            function setValue(newVal) {
                var step = getStep();
                var decimals = getDecimalPlaces(step);
                newVal = parseFloat(clamp(newVal).toFixed(decimals));
                input.value = newVal;
                // Fire input + change so jQuery Validate re-evaluates the field
                input.dispatchEvent(new Event('input', { bubbles: true }));
                input.dispatchEvent(new Event('change', { bubbles: true }));
            }

            btnMinus.addEventListener('click', function () {
                setValue((parseFloat(input.value) || 0) - getStep());
            });

            btnPlus.addEventListener('click', function () {
                setValue((parseFloat(input.value) || 0) + getStep());
            });
        });
    }

    // Scripts load at the end of <body>, so the DOM is already built.
    // The readyState guard covers any edge case where this script is
    // moved earlier in the page.
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initSpinners);
    } else {
        initSpinners();
    }
}());
