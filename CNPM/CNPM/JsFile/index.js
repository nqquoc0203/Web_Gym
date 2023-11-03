const progressCircle = document.querySelector(".autoplay-progress svg");
const progressContent = document.querySelector(".autoplay-progress span");
var swiper = new Swiper(".mySwiper", {
    spaceBetween: 30,
    centeredSlides: true,
    autoplay: {
        delay: 2500,
        disableOnInteraction: false
    },
    pagination: {
        el: ".swiper-pagination",
        clickable: true
    },
    navigation: {
        nextEl: ".swiper-button-next",
        prevEl: ".swiper-button-prev"
    },
    on: {
        autoplayTimeLeft(s, time, progress) {
            progressCircle.style.setProperty("--progress", 1 - progress);
            progressContent.textContent = `${Math.ceil(time / 1000)}s`;
        }
    }
});

function showSubcategory(subcategoryId) {
    // Ẩn tất cả danh mục con trước khi hiển thị danh mục con cụ thể
    hideAllSubcategories();

    // Hiển thị danh mục con được chỉ định
    var subcategoryElement = document.getElementById('subcategory' + subcategoryId);
    if (subcategoryElement) {
        subcategoryElement.style.display = 'block';
    }
}

function hideAllSubcategories() {
    // Ẩn tất cả các danh mục con
    var subcategories = document.querySelectorAll('[id^="subcategory"]');
    subcategories.forEach(function (subcategory) {
        subcategory.style.display = 'none';
    });
}
