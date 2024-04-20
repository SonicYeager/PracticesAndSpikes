fn circle_area_inside_square(square_area: f64) -> f64 {
    let side_length = square_area.sqrt();
    let radius = side_length / 2.0;
    std::f64::consts::PI * radius.powi(2)
}

fn assert_close(a:f64, b:f64, epsilon:f64) {
    assert!( (a-b).abs() < epsilon, "Expected: {}, got: {}",b,a);
}

#[test]
fn returns_expected() {
    assert_close(square_area_to_circle(9.0), 7.0685834705770345, 1e-8);
    assert_close(square_area_to_circle(20.0), 15.70796326794897, 1e-8);
}