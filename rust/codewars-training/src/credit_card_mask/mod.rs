/// Return a String with all characters masked as '#' except the last 4.
pub fn maskify(cc: &str) -> String
{
    let mask_length = cc.len().saturating_sub(4);
    "#".repeat(mask_length) + &cc[mask_length..]
}

#[cfg(test)]
mod credit_card_mask_tests
{
    // Note this useful idiom: importing names from outer (for mod tests) scope.
    use super::*;

    #[test]
    fn it_masks_example_strings() {
        assert_eq!(maskify("4556364607935616"), "############5616");
        assert_eq!(maskify("1"), "1");
        assert_eq!(maskify("11111"), "#1111");
    }
}