require 'test_helper'

class ProductEnvironmentsControllerTest < ActionController::TestCase
  setup do
    @product_environment = product_environments(:one)
  end

  test "should get index" do
    get :index
    assert_response :success
    assert_not_nil assigns(:product_environments)
  end

  test "should get new" do
    get :new
    assert_response :success
  end

  test "should create product_environment" do
    assert_difference('ProductEnvironment.count') do
      post :create, product_environment: { product_id: @product_environment.product_id, test_environment_id: @product_environment.test_environment_id }
    end

    assert_redirected_to product_environment_path(assigns(:product_environment))
  end

  test "should show product_environment" do
    get :show, id: @product_environment
    assert_response :success
  end

  test "should get edit" do
    get :edit, id: @product_environment
    assert_response :success
  end

  test "should update product_environment" do
    patch :update, id: @product_environment, product_environment: { product_id: @product_environment.product_id, test_environment_id: @product_environment.test_environment_id }
    assert_redirected_to product_environment_path(assigns(:product_environment))
  end

  test "should destroy product_environment" do
    assert_difference('ProductEnvironment.count', -1) do
      delete :destroy, id: @product_environment
    end

    assert_redirected_to product_environments_path
  end
end
