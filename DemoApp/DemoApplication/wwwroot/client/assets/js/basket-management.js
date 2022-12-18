const cookieObj = new URLSearchParams(document.cookie.replaceAll("&", "%26").replaceAll("; ", "&"))
var color = cookieObj.get("products")
console.log(color)



//$("add-product-to-basket-btn").click(function (e) {
//    e.preventdefault()
//    alert("salam")
//}



let btns = document.querySelectorAll(".add-product-to-basket-btn")

btns.forEach(x => x.addEventListener("click", function (e)
{
    e.preventDefault()
    fetch(e.target.href)
        .then(response => response.text())
        .then(data => {
            $('.cart-block').html(data);
        })
}))


//let removeBtns = document.querySelectorAll(".remove-product-to-basket-btn")

//removeBtns.forEach(x => x.addEventListener("click", function (e) {
//    e.preventDefault()

//    fetch(e.target.href)
//        .then(response => response.text())
//        .then(data => {
//            $('.cart-block').html(data);
//        })
//}))
$(document).on("click", ".remove-product-to-basket-btn", function (e)
{
    e.preventDefault();

    fetch(e.target.href)
        .then(response => response.text())
        .then(data => {
            $('.cart-block').html(data);
        })
})

$(document).on("click", ".show-book-modal", function (e) {
    e.preventDefault();

    var url = e.target.href;
    console.log(url)

    fetch(url)
        .then(response => response.text())
        .then(data => {
            $('.product-details-modal').html(data);
            console.log(data)
        })

    $("#quickModal").modal("show");
})

