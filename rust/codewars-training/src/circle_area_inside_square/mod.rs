/// 
/// 
/// # Arguments 
/// 
/// * `square_area`: 
/// 
/// returns: f64 
/// 
/// # Examples 
/// 
/// ```
/// 
/// ```
pub fn circle_area_inside_square(square_area: f64) -> f64 {
    let side_length = square_area.sqrt();
    let radius = side_length / 2.0;
    std::f64::consts::PI * radius.powi(2)
}

/// 
/// 
/// # Arguments 
/// 
/// * `a`: 
/// * `b`: 
/// * `epsilon`: 
/// 
/// returns: () 
/// 
/// # Examples 
/// 
/// ```
/// 
/// ```
pub fn assert_close(a: f64, b: f64, epsilon: f64) {
    assert!((a - b).abs() < epsilon, "Expected: {}, got: {}", b, a);
}

#[cfg(test)]
mod circle_area_inside_square_tests
{
    // Note this useful idiom: importing names from outer (for mod tests) scope.
    use super::*;

    #[test]
    fn circle_area_inside_square_test() {
        assert_close(circle_area_inside_square(9.0), 7.0685834705770345, 1e-8);
        assert_close(circle_area_inside_square(20.0), 15.70796326794897, 1e-8);
    }
}