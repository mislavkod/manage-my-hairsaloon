/* date-picker.js — Barbershop themed calendar component
   Handles date and datetime modes.
   Display format auto-detects: Croatian (hr-*) vs English.
   No external dependencies. */
(function () {
    'use strict';

    var locale = navigator.language || 'en-US';
    var isCroatian = locale.startsWith('hr');

    var DAYS_EN = ['Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa', 'Su'];
    var DAYS_HR = ['Po', 'Ut', 'Sr', 'Če', 'Pe', 'Su', 'Ne'];
    var DAYS = isCroatian ? DAYS_HR : DAYS_EN;

    /* ── helpers ──────────────────────────────── */

    function parseIsoDate(str) {
        if (!str) return null;
        // Accept "yyyy-MM-dd" and "yyyy-MM-ddTHH:mm"
        return str.length === 10 ? new Date(str + 'T00:00:00') : new Date(str);
    }

    function toIsoDate(d) {
        return d.getFullYear() +
            '-' + String(d.getMonth() + 1).padStart(2, '0') +
            '-' + String(d.getDate()).padStart(2, '0');
    }

    function toIsoDateTime(d) {
        return toIsoDate(d) +
            'T' + String(d.getHours()).padStart(2, '0') +
            ':' + String(d.getMinutes()).padStart(2, '0');
    }

    function formatDisplay(date, mode) {
        if (!date) return isCroatian ? 'Odaberi datum' : 'Select a date';
        if (isCroatian) {
            var d  = String(date.getDate()).padStart(2, '0');
            var mo = String(date.getMonth() + 1).padStart(2, '0');
            var y  = date.getFullYear();
            if (mode === 'datetime') {
                var h  = String(date.getHours()).padStart(2, '0');
                var mi = String(date.getMinutes()).padStart(2, '0');
                return d + '.' + mo + '.' + y + '. ' + h + ':' + mi;
            }
            return d + '.' + mo + '.' + y + '.';
        }
        var opts = mode === 'datetime'
            ? { year: 'numeric', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit' }
            : { year: 'numeric', month: 'long', day: 'numeric' };
        return date.toLocaleDateString(locale, opts);
    }

    function sameDay(a, b) {
        return a && b &&
            a.getFullYear() === b.getFullYear() &&
            a.getMonth()    === b.getMonth()    &&
            a.getDate()     === b.getDate();
    }

    /* ── init one datepicker ────────────────────── */

    function initDatePicker(wrap) {
        var mode        = wrap.dataset.dpMode || 'date';
        var minDate     = parseIsoDate(wrap.dataset.dpMin);
        var maxDate     = parseIsoDate(wrap.dataset.dpMax);
        var autoSubmit  = wrap.dataset.dpAutosubmit === 'true';
        var oncommit    = wrap.dataset.dpOncommit || '';

        var hidden      = wrap.querySelector('.dp-hidden');
        var trigger     = wrap.querySelector('.dp-trigger');
        var display     = wrap.querySelector('.dp-display');
        var popup       = wrap.querySelector('.dp-popup');
        var prevBtn     = wrap.querySelector('.dp-prev');
        var nextBtn     = wrap.querySelector('.dp-next');
        var monthLabel  = wrap.querySelector('.dp-month-year');
        var daysGrid    = wrap.querySelector('.dp-days');
        var weekdaysRow = wrap.querySelector('.dp-weekdays');
        var timeSection = wrap.querySelector('.dp-time-section');
        var hourInput   = wrap.querySelector('.dp-hour');
        var minInput    = wrap.querySelector('.dp-min');
        var confirmBtn  = wrap.querySelector('.dp-confirm');

        // Current navigation month
        var nav      = new Date();
        var selected = parseIsoDate(hidden.value);
        if (selected) nav = new Date(selected);

        // Render static weekday row once
        DAYS.forEach(function (d) {
            var el = document.createElement('div');
            el.className = 'dp-wday';
            el.textContent = d;
            weekdaysRow.appendChild(el);
        });

        /* ── render ── */

        function renderCalendar() {
            var year  = nav.getFullYear();
            var month = nav.getMonth();

            monthLabel.textContent = nav.toLocaleDateString(locale, {
                month: 'long', year: 'numeric'
            });

            daysGrid.innerHTML = '';

            // Start week on Monday (JS Sunday=0 → offset (day+6)%7)
            var firstDay    = new Date(year, month, 1).getDay();
            var startOffset = (firstDay + 6) % 7;
            var totalDays   = new Date(year, month + 1, 0).getDate();
            var today       = new Date();

            for (var i = 0; i < startOffset; i++) {
                var blank = document.createElement('div');
                blank.className = 'dp-cell dp-blank';
                daysGrid.appendChild(blank);
            }

            for (var day = 1; day <= totalDays; day++) {
                var cellDate   = new Date(year, month, day);
                var cell       = document.createElement('button');
                var isDisabled = (minDate && cellDate < minDate) || (maxDate && cellDate > maxDate);
                var isSelected = sameDay(selected, cellDate);
                var isToday    = sameDay(today, cellDate);

                cell.type      = 'button';
                cell.className = 'dp-cell';
                cell.textContent = day;

                if (isDisabled) {
                    cell.disabled = true;
                    cell.classList.add('dp-disabled');
                } else {
                    (function (cd) {
                        cell.addEventListener('click', function () { selectDay(cd); });
                    }(cellDate));
                }
                if (isSelected)         cell.classList.add('dp-selected');
                if (isToday && !isSelected) cell.classList.add('dp-today');

                daysGrid.appendChild(cell);
            }
        }

        /* ── selection ── */

        function commit() {
            if (!selected) return;
            if (mode === 'datetime') {
                selected.setHours(parseInt(hourInput.value, 10)  || 0);
                selected.setMinutes(parseInt(minInput.value, 10) || 0);
                hidden.value = toIsoDateTime(selected);
            } else {
                hidden.value = toIsoDate(selected);
            }
            display.textContent = formatDisplay(selected, mode);
            // Named callback — most reliable cross-browser trigger
            if (oncommit && typeof window[oncommit] === 'function') {
                window[oncommit](hidden.value);
            }
            // Also dispatch a change event for any additional listeners
            hidden.dispatchEvent(new Event('change', { bubbles: true }));
            if (autoSubmit) wrap.closest('form') && wrap.closest('form').submit();
        }

        function selectDay(date) {
            selected = selected ? new Date(selected) : new Date();
            selected.setFullYear(date.getFullYear(), date.getMonth(), date.getDate());
            nav = new Date(selected);
            renderCalendar();

            if (mode === 'date') {
                popup.hidden = true;
                commit();
            } else {
                // datetime mode: show time picker and immediately commit with current time.
                // User can refine the time and click OK to commit again.
                if (timeSection) timeSection.hidden = false;
                if (hourInput)   hourInput.value = String(selected.getHours()).padStart(2, '0');
                if (minInput)    minInput.value  = String(selected.getMinutes()).padStart(2, '0');
                commit();
            }
        }

        /* ── time inputs (datetime mode) ── */

        if (hourInput) {
            hourInput.addEventListener('change', function () {
                var v = Math.max(0, Math.min(23, parseInt(this.value, 10) || 0));
                this.value = String(v).padStart(2, '0');
                if (selected) { selected.setHours(v); commit(); }
            });
        }
        if (minInput) {
            minInput.addEventListener('change', function () {
                var v = Math.max(0, Math.min(59, parseInt(this.value, 10) || 0));
                this.value = String(v).padStart(2, '0');
                if (selected) { selected.setMinutes(v); commit(); }
            });
        }
        if (confirmBtn) {
            confirmBtn.addEventListener('click', function () {
                commit();
                popup.hidden = true;
            });
        }

        /* ── navigation ── */

        prevBtn.addEventListener('click', function () {
            nav.setMonth(nav.getMonth() - 1);
            renderCalendar();
        });
        nextBtn.addEventListener('click', function () {
            nav.setMonth(nav.getMonth() + 1);
            renderCalendar();
        });

        /* ── open / close ── */

        trigger.addEventListener('click', function (e) {
            e.stopPropagation();
            popup.hidden = !popup.hidden;
            if (!popup.hidden) {
                // Re-sync internal state from the hidden input every time the popup opens.
                // This handles external resets (e.g. "Clear dates" button) and keeps
                // selected consistent with what the hidden value actually holds.
                selected = parseIsoDate(hidden.value);
                if (selected) nav = new Date(selected);
                if (timeSection) {
                    if (selected) {
                        timeSection.hidden = false;
                        hourInput.value = String(selected.getHours()).padStart(2, '0');
                        minInput.value  = String(selected.getMinutes()).padStart(2, '0');
                    } else {
                        timeSection.hidden = true;
                    }
                }
                renderCalendar();
            }
        });

        document.addEventListener('click', function (e) {
            if (!wrap.contains(e.target)) popup.hidden = true;
        });

        // Keyboard: Escape closes
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') popup.hidden = true;
        });

        /* ── initial display ── */
        display.textContent = formatDisplay(selected, mode);
    }

    /* ── boot ────────────────────────────────── */

    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('.dp-wrap').forEach(function (w) {
            initDatePicker(w);
        });
    });
}());
