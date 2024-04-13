#include "high_and_low/kata.hpp"
#include <set>
#include <sstream>

namespace high_and_low
{
    auto get_high_and_low(const std::string& numbers) -> std::string
    {
        std::set<int> nums;
        std::istringstream iss(numbers);
        std::string number;
        while (std::getline(iss, number, ' ')) {
            nums.insert(std::stoi(number));
        }
        return std::to_string(*nums.rbegin()) + " " + std::to_string(*nums.begin());
    }
}