#include "digital_root/kata.hpp"

namespace digital_root
{
    auto get_digital_root(int Z) -> int
    {
        return --Z % 9 + 1;
    }
}
