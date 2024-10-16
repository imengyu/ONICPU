function tick() {
    io.P4 = (io.P0 < 15 || io.P0 > 35) && io.P1 > 1000;
}
