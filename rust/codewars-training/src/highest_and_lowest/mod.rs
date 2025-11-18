/// Finds the highest and lowest numbers in a string of space-separated numbers.
///
/// # Arguments
///
/// * `numbers` - A string of space-separated numbers.
///
/// # Returns
///
/// A string containing the highest and lowest numbers, separated by a space.
pub fn high_and_low(numbers: &str) -> String {
    let mut nums: Vec<i32> = numbers
        .split_whitespace()
        .filter_map(|s| s.parse().ok())
        .collect();

    if nums.is_empty() {
        return String::new();
    }

    nums.sort_unstable();
    let min = nums[0];
    let max = nums[nums.len() - 1];

    format!("{} {}", max, min)
}


#[cfg(test)]
mod highest_and_lowest_tests
{
    use super::*;

    #[test]
    fn example_test_1() {
        assert_eq!("42 -9", high_and_low("8 3 -5 42 -1 0 0 -9 4 7 4 -4"));
    }

    #[test]
    fn example_test_2() {
        assert_eq!("3 1", high_and_low("1 2 3"));
    }
}
