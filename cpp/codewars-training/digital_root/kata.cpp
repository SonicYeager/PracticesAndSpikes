#include "digital_root/kata.hpp"

namespace digital_root
{
    /**
     * @brief      Calculates the digital root of a number.
     *
     * @param[in]  Z     The input number.
     *
     * @return     The digital root.
     */
    auto get_digital_root(int Z) -> int
    {
        return --Z % 9 + 1;
    }
}
