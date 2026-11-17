// MVC5 hiển thị form-validation phía server. JS chỉ cải thiện UX, không thay thế kiểm tra server.
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('input[type=number][min]').forEach(function (el) {
        el.addEventListener('change', function () {
            var min = Number(el.getAttribute('min'));
            if (Number(el.value) < min) el.value = min;
        });
    });
});
