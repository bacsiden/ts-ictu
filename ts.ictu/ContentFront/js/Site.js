$(document).ready(function () {
  $('[data-toggle="offcanvas"]').click(function () {
    $('.row-offcanvas').toggleClass('active')
  });
  $("> ul > li", "#navbar").hover(function () {
      $(this).find("ul").slideDown();
  }, function () {
      $(this).find("ul").stop().slideUp();
  });
});